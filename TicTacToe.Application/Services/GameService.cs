using System;
using System.Data;
using System.Threading.Tasks;
using TicTacToe.Core.Configuration;
using TicTacToe.Core.DTOs;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions;
using TicTacToe.Core.Exceptions.Game;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Interfaces.Repositories;
using TicTacToe.Core.Interfaces.Services;
using TicTacToe.Core.Entities;
using TicTacToe.Application.Mappers;
using System.Collections.Specialized;

namespace TicTacToe.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IRandomizer _randomizer;
        private readonly GameSettings _gameSettings;
        private readonly IEtagGenerator _etagGenerator;
        private readonly int[] _dcol = new int[] {-1, 0, -1,  1 };
        private readonly int[] _drow = new int[] { 0, 1, -1, -1  };

        public GameService(IGameRepository gameRepository, IRandomizer randomizer, GameSettings gameSettings, IEtagGenerator etagGenerator)
        {
            _gameRepository = gameRepository;
            _randomizer = randomizer;
            _gameSettings = gameSettings;
            _etagGenerator = etagGenerator;
        }

        public async Task<GameDTO> CreateGameAsync(CreateGameDTO dto, CancellationToken cancellationToken = default)
        {
            var game = new Game(_gameSettings.BoardSize, dto.FirstPlayer, dto.SecondPlayer, _gameSettings.WinCon);
           
           await _gameRepository.CreateAsync(game, cancellationToken);

            return game.MapToDto();
        }

        public async Task<GameDTO> GetGameAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var game = await _gameRepository.GetByIdAsync(id, cancellationToken);
            if (game == null)
            {
                throw new GameNotFoundException(id);
            }
            return game.MapToDto();
        }

        //TODO:
        //сделать норм обработку/проверку ходов на выйгрыш после каждого хода с использованием массива направлений
        //контроллеры с IfMatch заголовком в который помещался бы Etag при создании новой игры, а при ходе игрока сверялся бы  

        public async Task<(MoveResultDTO? result, string etag)> MakeMoveAsync(MakeMoveDTO dto, string expectedVersion, CancellationToken cancellationToken=default)
        {
            var game = await _gameRepository.GetByIdAsync(dto.GameId , cancellationToken);
            
            if (game == null)
            {
                throw new GameNotFoundException(dto.GameId);
            }
            //try catch в контроллере вернуть Ok(Etag)
            if (game.Etag != expectedVersion)
            {
                throw new ConcurrencyException();
            }
            if (game.State != GameState.InProgress)
                throw new BadRequestException("Game is already finished");

            if (dto.Row < 0 || dto.Row >= game.BoardSize || dto.Column < 0 || dto.Column >= game.BoardSize)
                throw new InvalidMoveException(dto.Row,dto.Column);
            if (dto.Player != game.CurrentPlayer)
                throw new BadRequestException("It`s not your turn");

            if (game.Board[dto.Row][dto.Column] != ' ')
                throw new InvalidMoveException(dto.Row,dto.Column);

            //специальное правило
            if (_randomizer.ForceOpponentMoveRule(game)) 
            {
                game.Board[dto.Row][dto.Column] = game.CurrentPlayer=='X'? 'O': 'X';
            }
            else
            {
                game.Board[dto.Row][dto.Column] = game.CurrentPlayer;
            }

            //ход игрока
            Move? move = await _gameRepository.CreateMoveAsync(dto.GameId,dto.Row, dto.Column,dto.Player ,cancellationToken);
            var generatedEtag = _etagGenerator.GetEtag(move);
            game.MoveCount += 1;
            GameState check = CheckWin(game,move);
            bool isFull = IsBoardFull(game);
                        
            if (check==GameState.InProgress && isFull) {game.State = GameState.Draw;}
            else { game.State = check;}

            game.Etag = generatedEtag;
            move.Etag =generatedEtag;
            game.CurrentPlayer = game.CurrentPlayer == 'X' ? 'O' : 'X';
            await _gameRepository.UpdateAsync(game, cancellationToken);


            return ( new MoveResultDTO { state=game.State},generatedEtag);
        }

        

        private GameState CheckWin(Game game, Move move)
        {
            if (game?.Board == null || move == null)
                return GameState.InProgress;

            char player = game.CurrentPlayer;
            int size = _gameSettings.BoardSize;
            int winCon = _gameSettings.WinCon;

            for (int i = 0; i < _drow.Length; i++)
            {
                int count = 1;
                bool hasValidDirection = true;

                for (int step = 1; step < winCon && hasValidDirection; step++)
                {
                    bool foundMatch = false;

                   
                    int r1 = move.Row + _drow[i] * step;
                    int c1 = move.Column + _dcol[i] * step;
                    if (r1 >= 0 && r1 < size && c1 >= 0 && c1 < size)
                    {
                        if (game.Board[r1][c1] == player)
                        {
                            count++;
                            foundMatch = true;
                        }
                    }

                    int r2 = move.Row - _drow[i] * step;
                    int c2 = move.Column - _dcol[i] * step;
                    if (r2 >= 0 && r2 < size && c2 >= 0 && c2 < size)
                    {
                        if (game.Board[r2][c2] == player)
                        {
                            count++;
                            foundMatch = true;
                        }
                    }

                   
                    hasValidDirection = foundMatch;
                }

                if (count >= winCon)
                {
                    return player == 'X' ? GameState.XWon : GameState.OWon;
                }
            }

            return GameState.InProgress;
        }



        private bool IsBoardFull(Game game)
        {
            if (game.MoveCount == _gameSettings.BoardSize * _gameSettings.BoardSize) { return true; }
            return false;
        }
    }
}
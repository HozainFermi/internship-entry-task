using TicTacToe.Core.DTOs;
using TicTacToe.Core.Entities;


namespace TicTacToe.Application.Mappers;

public static class GameMapper
{
    public static GameDTO MapToDto(this Game game)
    {
        return new GameDTO
        {
            Id = game.Id,
            FirstPlayer = game.FirstPlayer,
            SecondPlayer = game.SecondPlayer,
            Board = game.Board,
            CurrentPlayer = game.CurrentPlayer,
            State = game.State,
            BoardSize = game.BoardSize,
            WinLength = game.WinCon,
            CreatedAt = game.CreatedAt,
            Etag = game.Etag,
            Moves = game.Moves.Select(m=>m.MapToDto()).ToList(),
        };
    }

    public static MoveDTO MapToDto(this Move move)
    {
        return new MoveDTO
        {
            Id = move.Id,
            Column = move.Column,
            Row = move.Row,
            PlayerName = move.Player,
          
            
        };
    }
}

using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Application.Services;
using TicTacToe.Application.Mappers;
using TicTacToe.Core.Configuration;
using TicTacToe.Core.DTOs;
using TicTacToe.Core.Entities;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Interfaces.Repositories;
using TicTacToe.Tests;
using TicTacToe.Core.Interfaces.Services;
using Xunit;

namespace TicTacToe.Tests.Unit
{
    public class GameLogicTests
    {
        [Theory]
        [MemberData(nameof(TestGameSettings.GetSettings), MemberType = typeof(TestGameSettings))]
        public async Task CreateGameAsync_ShouldReturnGameDto_ForDifferentSettings(GameSettings settings)
        {
            // Arrange
            var mockRepo = new Mock<IGameRepository>();
            var mockRandomizer = new Mock<IRandomizer>();
            var mockEtagGenerator = new Mock<IEtagGenerator>();

            var game = new Game(settings.BoardSize, "player1", "player2", settings.WinCon);
            var expectedDto = game.MapToDto();

            mockRepo.Setup(x => x.CreateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(game);

            // Используем реальный объект settings из TestGameSettings
            var service = new GameService(
                gameRepository: mockRepo.Object,
                randomizer: mockRandomizer.Object,
                gameSettings: settings, // Передаем реальные настройки
                etagGenerator: mockEtagGenerator.Object);

            var dto = new CreateGameDTO { FirstPlayer = "player1", SecondPlayer = "player2" };

            // Act
            var result = await service.CreateGameAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(settings.BoardSize, result.BoardSize);
            Assert.Equal(settings.WinCon, result.WinLength);
            mockRepo.Verify(x => x.CreateAsync(
                It.Is<Game>(g => g.BoardSize == settings.BoardSize && g.WinCon == settings.WinCon),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [MemberData(nameof(TestGameSettings.GetSettings), MemberType = typeof(TestGameSettings))]
        public void CheckWin_ShouldDetectWin_ForDifferentSettings(GameSettings settings)
        {
            // Arrange
            var mockRepo = new Mock<IGameRepository>();
            var mockRandomizer = new Mock<IRandomizer>();
            var mockEtagGenerator = new Mock<IEtagGenerator>();

            var service = new GameService(
                gameRepository: mockRepo.Object,
                randomizer: mockRandomizer.Object,
                gameSettings: settings, // Используем реальные настройки
                etagGenerator: mockEtagGenerator.Object);

            // Создаем доску с выигрышной комбинацией
            var board = new char[settings.BoardSize][];
            for (int i = 0; i < settings.BoardSize; i++)
            {
                board[i] = new char[settings.BoardSize];
                Array.Fill(board[i], ' ');
            }

            // Заполняем выигрышную линию
            for (int i = 0; i < settings.WinCon; i++)
            {
                board[i][0] = 'X';
            }

            var game = new Game(settings.BoardSize, "p1", "p2", settings.WinCon)
            {
                Board = board,
                CurrentPlayer = 'X'
            };

            var move = new Move { Row = 0, Column = 0, Player = 'X' };

            // Act
            var result = service.CheckWin(game, move);

            // Assert
            Assert.Equal(GameState.XWon, result);
        }

        [Theory]
        [MemberData(nameof(TestGameSettings.GetSettings), MemberType = typeof(TestGameSettings))]
        public async Task MakeMoveAsync_ShouldHandleDifferentBoardSizes(GameSettings settings)
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var mockRepo = new Mock<IGameRepository>();
            var mockRandomizer = new Mock<IRandomizer>();
            var mockEtagGenerator = new Mock<IEtagGenerator>();

            var board = new char[settings.BoardSize][];
            for (int i = 0; i < settings.BoardSize; i++)
            {
                board[i] = new char[settings.BoardSize];
                Array.Fill(board[i], ' ');
            }

            var game = new Game(settings.BoardSize, "player1", "player2", settings.WinCon)
            {
                Id = gameId,
                Etag = "old-etag",
                State = GameState.InProgress,
                CurrentPlayer = 'X',
                Board = board
            };

            mockRepo.Setup(x => x.GetByIdAsync(gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(game);

            
            mockRepo.Setup(x => x.UpdateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Game g, CancellationToken _) => g);


            mockRepo.Setup(x => x.CreateMoveAsync(
                It.IsAny<Guid>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<char>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Move());

            mockEtagGenerator.Setup(x => x.GetEtag(It.IsAny<Move>()))
                .Returns("new-etag");

            var service = new GameService(
                gameRepository: mockRepo.Object,
                randomizer: mockRandomizer.Object,
                gameSettings: settings,
                etagGenerator: mockEtagGenerator.Object);

            // Выбираем центральную клетку
            var row = settings.BoardSize / 2;
            var col = settings.BoardSize / 2;
            var dto = new MakeMoveDTO
            {
               
                Player = 'X',
                Row = row,
                Column = col
            };

            // Act
            var result = await service.MakeMoveAsync(gameId, dto, "old-etag");

            // Assert
            Assert.Equal("new-etag", result.etag);
            Assert.Equal(GameState.InProgress, result.result.state);
            Assert.Equal('X', game.Board[row][col]);
        }

        [Fact]
        public async Task CreateGameAsync_ShouldReturnNull_WhenCreationFails()
        {
            // Arrange
            var mockRepo = new Mock<IGameRepository>();
            var settings = new GameSettings(3, 3); // Реальные настройки

            mockRepo.Setup(x => x.CreateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Game)null);

            var service = new GameService(
                gameRepository: mockRepo.Object,
                randomizer: Mock.Of<IRandomizer>(),
                gameSettings: settings,
                etagGenerator: Mock.Of<IEtagGenerator>());

            var dto = new CreateGameDTO { FirstPlayer = "player1", SecondPlayer = "player2" };

            // Act
            var result = await service.CreateGameAsync(dto);

            // Assert
            Assert.Null(result);
            mockRepo.Verify(x => x.CreateAsync(It.IsAny<Game>(), It.IsAny<CancellationToken>()), Times.Once);
        }




    }
}

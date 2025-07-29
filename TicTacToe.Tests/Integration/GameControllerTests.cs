using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System.Net;
using System.Net.Http.Json;
using TicTacToe.API;
using TicTacToe.Core.DTOs;
using TicTacToe.Core.Enums;
using TicTacToe.Core.Exceptions.Game;
using TicTacToe.Core.Interfaces.Services;

namespace TicTacToe.Tests.Integration
{
    public class GamesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IGameService> _mockGameService = new();

        public GamesControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["GAME_BOARD_SIZE"] = "4",
                        ["GAME_WIN_CONDITION"] = "3"
                    });
                });

                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IGameService>(_mockGameService.Object);
                });
            });
        }

        private HttpClient CreateClient()
        {
            return _factory.CreateClient();
        }

        [Fact]
        public async Task CreateGameAsync_ReturnsCreated_WhenGameCreated()
        {
            // Arrange
            var client = CreateClient();
            var dto = new CreateGameDTO { FirstPlayer = "Player1", SecondPlayer = "Player2" };
            var expectedGame = new GameDTO
            {
                Id = Guid.NewGuid(),
                FirstPlayer = "Player1",
                SecondPlayer = "Player2",
                BoardSize = 3,
                WinLength = 3
            };

            _mockGameService.Setup(x => x.CreateGameAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedGame);

            // Act
            var response = await client.PostAsJsonAsync("/games", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<dynamic>();
            Assert.Equal(expectedGame.Id.ToString(), content?.game?.id?.ToString());
        }

        [Fact]
        public async Task CreateGameAsync_ReturnsBadRequest_WhenInvalidData()
        {
            // Arrange
            var client = CreateClient();
            var invalidDto = new CreateGameDTO { FirstPlayer = "", SecondPlayer = "" };

            // Act
            var response = await client.PostAsJsonAsync("/games", invalidDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetGameAsync_ReturnsGame_WhenExists()
        {
            // Arrange
            var client = CreateClient();
            var gameId = Guid.NewGuid();
            var expectedGame = new GameDTO
            {
                Id = gameId,
                Etag = "test-etag",
                FirstPlayer = "Player1",
                SecondPlayer = "Player2"
            };

            _mockGameService.Setup(x => x.GetGameAsync(gameId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedGame);

            // Act
            var response = await client.GetAsync($"/games/{gameId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("\"test-etag\"", response.Headers.ETag?.Tag);

            var game = await response.Content.ReadFromJsonAsync<GameDTO>();
            Assert.Equal(gameId, game?.Id);
        }

        [Fact]
        public async Task GetGameAsync_ReturnsNotFound_WhenNotExists()
        {
            // Arrange
            var client = CreateClient();
            var gameId = Guid.NewGuid();

            _mockGameService.Setup(x => x.GetGameAsync(gameId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new GameNotFoundException(gameId));

            // Act
            var response = await client.GetAsync($"/games/{gameId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task MakeMoveAsync_ReturnsOk_WhenValidMove()
        {
            // Arrange
            var client = CreateClient();
            var gameId = Guid.NewGuid();
            var moveDto = new MakeMoveDTO
            {

                Player = 'X',
                Row = 0,
                Column = 0
            };
            var expectedResult = new MoveResultDTO { state = GameState.InProgress };

            _mockGameService.Setup(x => x.MakeMoveAsync(
                gameId,
                It.IsAny<MakeMoveDTO>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((expectedResult, "new-etag"));

            client.DefaultRequestHeaders.Add("If-Match", "old-etag");

            // Act
            var response = await client.PostAsJsonAsync($"/games/{gameId}/moves", moveDto);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("\"new-etag\"", response.Headers.ETag?.Tag);
        }

        [Fact]
        public async Task MakeMoveAsync_ReturnsConcurrencyError_WhenEtagMismatch()
        {
            // Arrange
            var client = CreateClient();
            var gameId = Guid.NewGuid();
            var moveDto = new MakeMoveDTO
            {

                Player = 'X',
                Row = 0,
                Column = 0
            };

            _mockGameService.Setup(x => x.MakeMoveAsync(
                gameId,
                It.IsAny<MakeMoveDTO>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConcurrencyException());

            client.DefaultRequestHeaders.Add("If-Match", "wrong-etag");

            // Act
            var response = await client.PostAsJsonAsync($"/games/{gameId}/moves", moveDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal("wrong-etag", content);
        }




    }
}
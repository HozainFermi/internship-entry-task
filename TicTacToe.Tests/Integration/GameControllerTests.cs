
//    using global::TicTacToe.Core.DTOs;
//    using global::TicTacToe.Core.Enums;
//    using global::TicTacToe.Core.Exceptions.Game;
//    using global::TicTacToe.Core.Interfaces.Services;
//    using Microsoft.AspNetCore.Mvc.Testing;
//    using Microsoft.Extensions.DependencyInjection;
//using Microsoft.VisualStudio.TestPlatform.TestHost;
//using Moq;
//    using System.Net;
//    using System.Net.Http.Headers;
//    using System.Net.Http.Json;    
//    using TicTacToe.Core.Configuration;
//    using TicTacToe.Core.DTOs;
//    using TicTacToe.Core.Interfaces.Services;
//    using Xunit;

//    namespace TicTacToe.Tests.Integration
//    {
//        public class GamesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
//        {
//            private readonly WebApplicationFactory<Program> _factory;
//            private  Mock<IGameService> _mockGameService;

//            public GamesControllerIntegrationTests(WebApplicationFactory<Program> factory)
//            {
//                _factory = factory.WithWebHostBuilder(builder =>
//                {
//                    builder.ConfigureServices(services =>
//                    {
                        
//                        var descriptor = services.SingleOrDefault(
//                            d => d.ServiceType == typeof(IGameService));

//                        if (descriptor != null)
//                        {
//                            services.Remove(descriptor);
//                        }

//                        _mockGameService = new Mock<IGameService>();
//                        services.AddSingleton(_mockGameService.Object);
//                    });
//                });
//            }

//            [Fact]
//            public async Task CreateGameAsync_ReturnsCreated_WhenGameCreated()
//            {
//                // Arrange
//                var client = _factory.CreateClient();
//                var dto = new CreateGameDTO { FirstPlayer = "Player1", SecondPlayer = "Player2" };
//                var expectedGame = new GameDTO
//                {
//                    Id = Guid.NewGuid(),
//                    FirstPlayer = "Player1",
//                    SecondPlayer = "Player2",
//                    BoardSize = 3,
//                    WinLength = 3
//                };

//                _mockGameService.Setup(x => x.CreateGameAsync(dto, It.IsAny<CancellationToken>()))
//                    .ReturnsAsync(expectedGame);

//                // Act
//                var response = await client.PostAsJsonAsync("/games", dto);

//                // Assert
//                response.EnsureSuccessStatusCode();
//                Assert.Equal(HttpStatusCode.Created, response.StatusCode);

//                var content = await response.Content.ReadFromJsonAsync<dynamic>();
//                Assert.NotNull(content);
//                Assert.Equal(expectedGame.Id.ToString(), content?.game?.Id?.ToString());
//            }

//            [Fact]
//            public async Task GetGameAsync_ReturnsOkWithEtag_WhenGameExists()
//            {
//                // Arrange
//                var client = _factory.CreateClient();
//                var gameId = Guid.NewGuid();
//                var expectedGame = new GameDTO
//                {
//                    Id = gameId,
//                    Etag = "test-etag",
//                    FirstPlayer = "Player1",
//                    SecondPlayer = "Player2"
//                };

//                _mockGameService.Setup(x => x.GetGameAsync(gameId, It.IsAny<CancellationToken>()))
//                    .ReturnsAsync(expectedGame);

//                // Act
//                var response = await client.GetAsync($"/games/{gameId}");

//                // Assert
//                response.EnsureSuccessStatusCode();
//                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

//                var etagHeader = response.Headers.ETag;
//                Assert.NotNull(etagHeader);
//                Assert.Equal(expectedGame.Etag, etagHeader.Tag);

//                var game = await response.Content.ReadFromJsonAsync<GameDTO>();
//                Assert.Equal(expectedGame.Id, game?.Id);
//            }

//            [Fact]
//            public async Task GetGameAsync_ReturnsNotFound_WhenGameNotExists()
//            {
//                // Arrange
//                var client = _factory.CreateClient();
//                var gameId = Guid.NewGuid();

//                _mockGameService.Setup(x => x.GetGameAsync(gameId, It.IsAny<CancellationToken>()))
//                    .ThrowsAsync(new GameNotFoundException(gameId));

//                // Act
//                var response = await client.GetAsync($"/games/{gameId}");

//                // Assert
//                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//            }

//            [Fact]
//            public async Task MakeMoveAsync_ReturnsOkWithEtag_WhenMoveIsValid()
//            {
//                // Arrange
//                var client = _factory.CreateClient();
//                var gameId = Guid.NewGuid();
//                var moveDto = new MakeMoveDTO
//                {
//                    GameId = gameId,
//                    Player = 'X',
//                    Row = 0,
//                    Column = 0
//                };
//                var expectedEtag = "new-etag";
//                var expectedResult = new MoveResultDTO { state = GameState.InProgress };

//                _mockGameService.Setup(x => x.MakeMoveAsync(moveDto, It.IsAny<string>(), It.IsAny<CancellationToken>()))
//                    .ReturnsAsync((expectedResult, expectedEtag));

//                // Добавляем If-Match заголовок
//                client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue("\"old-etag\""));

//                // Act
//                var response = await client.PostAsJsonAsync($"/games/{gameId}/moves", moveDto);

//                // Assert
//                response.EnsureSuccessStatusCode();
//                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

//                var etagHeader = response.Headers.ETag;
//                Assert.NotNull(etagHeader);
//                Assert.Equal(expectedEtag, etagHeader.Tag);
//            }

//            [Fact]
//            public async Task MakeMoveAsync_ReturnsConcurrencyConflict_WhenEtagMismatch()
//            {
//                // Arrange
//                var client = _factory.CreateClient();
//                var gameId = Guid.NewGuid();
//                var moveDto = new MakeMoveDTO
//                {
//                    GameId = gameId,
//                    Player = 'X',
//                    Row = 0,
//                    Column = 0
//                };
//                var expectedEtag = "old-etag";

//                _mockGameService.Setup(x => x.MakeMoveAsync(moveDto, expectedEtag, It.IsAny<CancellationToken>()))
//                    .ThrowsAsync(new ConcurrencyException());

//                // Добавляем If-Match заголовок
//                client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue($"\"{expectedEtag}\""));

//                // Act
//                var response = await client.PostAsJsonAsync($"/games/{gameId}/moves", moveDto);

//                // Assert
//                response.EnsureSuccessStatusCode();
//                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

//                var content = await response.Content.ReadAsStringAsync();
//                Assert.Equal(expectedEtag, content);
//            }

//            [Fact]
//            public async Task MakeMoveAsync_ReturnsBadRequest_WhenMoveIsInvalid()
//            {
//                // Arrange
//                var client = _factory.CreateClient();
//                var gameId = Guid.NewGuid();
//                var moveDto = new MakeMoveDTO
//                {
//                    GameId = gameId,
//                    Player = 'X',
//                    Row = -1, // Недопустимая строка
//                    Column = 0
//                };

//                _mockGameService.Setup(x => x.MakeMoveAsync(moveDto, It.IsAny<string>(), It.IsAny<CancellationToken>()))
//                    .ThrowsAsync(new InvalidMoveException(moveDto.Row, moveDto.Column));

//                // Act
//                var response = await client.PostAsJsonAsync($"/games/{gameId}/moves", moveDto);

//                // Assert
//                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//            }
//        }
//    }


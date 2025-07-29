using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TicTacToe.Core.Configuration;
using TicTacToe.Core.DTOs;
using TicTacToe.Core.Entities;
using TicTacToe.Infrastructure.DbContexts;

namespace TicTacToe.Tests.Integration
{
    public class GamesControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly GameDbContext _dbContext;
        private readonly HttpClient _client;

        public GamesControllerTests()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Удаляем регистрацию реального DbContext
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<GameDbContext>));

                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        // Добавляем InMemory Database
                        services.AddDbContext<GameDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDb");
                        });
                        using var scope = services.BuildServiceProvider().CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
                        db.Database.EnsureCreated();
                       
                        // Добавляем тестовые настройки
                        services.AddSingleton(new GameSettings(3, 3));
                    });
                });

            // Создаем экземпляр DbContext для тестов
            var scope = _factory.Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

            // Применяем миграции для InMemory
            _dbContext.Database.EnsureCreated();

            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
            _client.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task CreateGameAsync_ReturnsCreated_WhenValidRequest()
        {
            // Arrange
            var dto = new CreateGameDTO { FirstPlayer = "Player1", SecondPlayer = "Player2" };

            // Act
            var response = await _client.PostAsJsonAsync("/games", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var game = await _dbContext.Games.FirstOrDefaultAsync();
            Assert.NotNull(game);
            Assert.Equal("Player1", game.FirstPlayer);
        }

        [Fact]
        public async Task GetGameAsync_ReturnsGame_WhenExists()
        {
            // Arrange
            var testGame = new Game(3, "Player1", "Player2", 3);
            _dbContext.Games.Add(testGame);
            await _dbContext.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/games/{testGame.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var game = await response.Content.ReadFromJsonAsync<GameDTO>();
            Assert.Equal(testGame.Id, game?.Id);
        }

        [Fact]
        public async Task MakeMoveAsync_ReturnsOk_WhenValidMove()
        {
            // Arrange
            var testGame = new Game(3, "Player1", "Player2", 3) { CurrentPlayer = 'X' };
            _dbContext.Games.Add(testGame);
            await _dbContext.SaveChangesAsync();

            var moveDto = new MakeMoveDTO
            {
             
                Player = 'X',
                Row = 0,
                Column = 0
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/games/{testGame.Id}/moves", moveDto);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(1, await _dbContext.Moves.CountAsync());
        }
    }
}
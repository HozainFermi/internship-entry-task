using Microsoft.AspNetCore.Builder;
using TicTacToe.Application;
using TicTacToe.Core.Configuration;
using TicTacToe.Infrastructure;
using TicTacToe.Infrastructure.DbContexts;


namespace TicTacToe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Configuration.AddEnvironmentVariables("GAME_");
            var gameSettings = new GameSettings(builder.Configuration.GetValue<int>("GAME_BOARD_SIZE"), builder.Configuration.GetValue<int>("GAME_WIN_CONDITION") );
            builder.Services.AddSingleton(gameSettings);

            
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            
            builder.Services.AddCors(options =>
                options.AddPolicy("AllowAll",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()));

            var app = builder.Build();

          
            app.UseCors("AllowAll");

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<GameDbContext>();

                var retries = 5;
                while (retries-- > 0)
                {
                    try
                    {
                        dbContext.Database.EnsureCreated();
                        break;
                    }
                    catch (Exception)
                    {
                        if (retries == 0) throw;
                        Thread.Sleep(2000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                throw;
            }

            app.UseRouting();
            app.UseAuthorization(); 
            app.MapControllers();

            app.Run();
        }
    }
}

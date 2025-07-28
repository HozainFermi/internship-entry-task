using TicTacToe.Core.Configuration;
using TicTacToe.Infrastructure;
using TicTacToe.Application;


namespace TicTacToe
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddEnvironmentVariables("GAME_");
            var gameSettings = new GameSettings(builder.Configuration.GetValue<int>("BOARD_SIZE"),
                builder.Configuration.GetValue<int>("WIN_CONDITION"));
            builder.Services.AddSingleton(gameSettings);

            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

          var app = builder.Build();
            
            if (!app.Environment.IsDevelopment())
            {
                
                app.UseExceptionHandler("/Error");
                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.Run();
        }
    }
}

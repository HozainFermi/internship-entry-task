using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Interfaces.Repositories;
using TicTacToe.Infrastructure.DbContexts;
using TicTacToe.Infrastructure.Repositories;

namespace TicTacToe.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddSingleton<IRandomizer, Randomizer>();
            
            services.AddDbContext<GameDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}

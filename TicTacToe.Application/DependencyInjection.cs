using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Application.Services;
using TicTacToe.Core.Interfaces.Services;

namespace TicTacToe.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGameService, GameService>();
            services.AddSingleton<IEtagGenerator, ETagGenerator>();           
            
            return services;
        }
    }
}

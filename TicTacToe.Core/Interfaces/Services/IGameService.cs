using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.DTOs;


namespace TicTacToe.Core.Interfaces.Services
{
    public interface IGameService
    {
        public Task<GameDTO> CreateGameAsync(CreateGameDTO game, CancellationToken cancellationToken);      
        Task<(MoveResultDTO? result, string etag)> MakeMoveAsync(Guid gameId, MakeMoveDTO dto, string expectedVersion, CancellationToken cancellationToken);
        public Task<GameDTO> GetGameAsync(Guid gameId, CancellationToken cancellationToken);



    }
}

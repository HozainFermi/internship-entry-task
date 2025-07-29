using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Entities;

namespace TicTacToe.Core.Interfaces.Repositories
{
    public interface IGameRepository
    {
        public Task<Game?> CreateAsync(Game game, CancellationToken cancellationToken);
        //public Task<Game?> GetByIdAsync(Guid id, string expectedEtag, CancellationToken cancellationToken);
        public Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        public Task<Game?> UpdateAsync(Game game, CancellationToken cancellationToken);
        public Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken);

        public Task<Move?> CreateMoveAsync(Guid gameId, int row, int col, char playerChar, CancellationToken cancellationToken);
        public Task<Move?> GetMoveAsync(Guid moveId, CancellationToken cancellationToken);
    }
}

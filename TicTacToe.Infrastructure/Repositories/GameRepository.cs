using TicTacToe.Infrastructure.DbContexts;
using TicTacToe.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Core.Entities;
using TicTacToe.Core.Exceptions.Game;
using TicTacToe.Core.Interfaces.Services;

namespace TicTacToe.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GameDbContext _context;
        private readonly IEtagGenerator _etagGenerator;

        public GameRepository(GameDbContext context, IEtagGenerator etagGenerator)
        {
            _context = context;
            _etagGenerator = etagGenerator;
            
        }
        

        public async Task<Game?> CreateAsync(Game game, CancellationToken cancellationToken)
        {
            var found = await _context.Games.AsNoTracking().FirstOrDefaultAsync(g=>g.Id==game.Id, cancellationToken);
            if (cancellationToken.IsCancellationRequested) {return null;}
            if (found == null)
            {
                _context.Games.Add(game);
                await _context.SaveChangesAsync();
                return game;
            }
            else
            {
                return null;
            }
        }

        public async Task<Move?> CreateMoveAsync(Guid gameId, int row, int col, char player, CancellationToken cancellationToken)
        {
            var moveExists = _context.Moves.AnyAsync(m => m.GameId == gameId && m.Row == row && m.Column == col,cancellationToken);
            var foundGame = _context.Games.FindAsync(gameId,cancellationToken).AsTask();
            
            await Task.WhenAll(moveExists, foundGame);
            
            if (await moveExists) { return null; }
            if (foundGame.Result == null) { return null; }

            var createdMove = await  _context.Moves.AddAsync(new Move { GameId=gameId, Column=col, Row=row, Player=player, Etag=foundGame.Result.Etag}, cancellationToken);

           // foundGame.Result.Etag = _etagGenerator.GetEtag(createdMove.Entity);

            await _context.SaveChangesAsync();

            return createdMove.Entity;

        }

        public async Task<Move?> GetMoveAsync(Guid moveId, CancellationToken cancellationToken)
        {
            return await _context.Moves.FindAsync(moveId, cancellationToken);
        }

        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var found = await _context.Games.FindAsync(id, cancellationToken);
            if (found == null) { return false; }

            _context.Games.Remove(found);
            _context.SaveChanges();
            return true;
        }
        
        //public async Task<Game?> GetByIdAsync(Guid id, string expectedEtag ,CancellationToken cancellationToken)
        //{
           
        //    var found = await _context.Games.FirstOrDefaultAsync(g => g.Id == id,cancellationToken);
        //    if (cancellationToken.IsCancellationRequested) { return null;}

        //    if (found != null && expectedEtag != null && found.Etag != expectedEtag)
        //    {
        //        throw new ConcurrencyException();
        //    }

        //    if (found != null) { return found; }
        //    else { return null; }
        //}

        public async Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var found = await _context.Games.Include(g => g.Moves).FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
            if (cancellationToken.IsCancellationRequested) { return null; }
          
            if (found != null) { return found; }
            else { return null; }
        }

        public async Task<Game?> UpdateAsync(Game game,CancellationToken cancellationToken)
        {
            try
            {
                var existingGame = await _context.Games
                    .FirstOrDefaultAsync(g => g.Id == game.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (existingGame == null)
                {
                    return null;
                }

                _context.Entry(existingGame).CurrentValues.SetValues(game);
                _context.Entry(existingGame).Property(g => g.Board).IsModified = true;
                await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return existingGame; 
            }
            catch (OperationCanceledException)
            {
                
                return null; 
            }
            catch (DbUpdateException ex)
            {
                
                return null; 
            }
        }

        
    }
}
using TicTacToe.Core.Entities;
using TicTacToe.Core.Helpers;
using TicTacToe.Core.Interfaces.Services;

namespace TicTacToe.Application.Services
{
    public class ETagGenerator : IEtagGenerator
    {
        public string GetEtag(Move move)
        {
            var hash = HashCodeGenerator.GenerateHash(move.GameId, move.Row, move.Column);
            return hash;
        }
    }
}

using TicTacToe.Core.Entities;
using TicTacToe.Core.Interfaces;


namespace TicTacToe.Infrastructure
{
    public class Randomizer: IRandomizer
    {

        public bool ForceOpponentMoveRule(Game game)
        {
            if (game.MoveCount % 3 == 0 && Random.Shared.NextDouble() < 0.1)
            {
                return true;
            }

            return false;
        }

    }
}
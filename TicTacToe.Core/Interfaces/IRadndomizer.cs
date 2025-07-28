using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Entities;


namespace TicTacToe.Core.Interfaces
{
    public interface IRandomizer
    {
        public bool ForceOpponentMoveRule(Game game);
    }
}

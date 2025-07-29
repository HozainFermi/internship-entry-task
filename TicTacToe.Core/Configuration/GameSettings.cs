using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.Configuration
{
    public class GameSettings
    {
        public int BoardSize { get; set; }
        public int WinCon { get; set; }
        public GameSettings(int board, int wincon) 
        {
            if (board < 3)
                throw new ArgumentOutOfRangeException(nameof(board), "Board size must be at least 3");
            if (wincon < 3)
                throw new ArgumentOutOfRangeException(nameof(wincon), "Win condition must be at least 3");
            if (wincon > board)
                throw new ArgumentOutOfRangeException(nameof(wincon), "Win condition must be less or equal than boardSize");
            BoardSize = board;
            WinCon = wincon;
        }
    }
}

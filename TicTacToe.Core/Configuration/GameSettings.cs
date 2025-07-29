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
            BoardSize = board==0?3:board;
            WinCon = wincon==0?3:wincon;

            if (BoardSize < 3)
                throw new ArgumentOutOfRangeException(nameof(board), "Board size must be at least 3");
            if (WinCon < 3)
                throw new ArgumentOutOfRangeException(nameof(wincon), "Win condition must be at least 3");
            if (WinCon > BoardSize)
                throw new ArgumentOutOfRangeException(nameof(wincon), "Win condition must be less or equal than boardSize");
        }
    }
}

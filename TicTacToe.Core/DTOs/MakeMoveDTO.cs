using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.DTOs
{
    public class MakeMoveDTO
    {
        public Guid GameId { get; set; }
        public required char Player { get; set; } 
        public int Row { get; set; }
        public int Column { get; set; }

    }
}

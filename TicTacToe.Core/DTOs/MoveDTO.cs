using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.DTOs
{
    public class MoveDTO
    {       
            public Guid Id { get; set; }
            
            public char PlayerName { get; set; }
            
            public int Row { get; set; }

            public int Column { get; set; }
        
    }
}

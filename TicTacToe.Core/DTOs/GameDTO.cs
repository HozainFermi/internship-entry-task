using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Enums;

namespace TicTacToe.Core.DTOs
{
    public class GameDTO
    {
        public Guid Id { get; set; }

        public string FirstPlayer { get; set; } = null!;

        public char FirstPlayerSymbol { get; set; } = 'X';

        public string SecondPlayer { get; set; } = null!;

        public char SecondPlayerSymbol { get; set; } = 'O';

        public char CurrentPlayer { get; set; } = 'X';

        public int BoardSize { get; set; }

        public int WinLength { get; set; }

        public GameState State { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Etag { get; set; } = null!;       

        public char[][]? Board { get; set; }
        
        public List<MoveDTO> Moves { get; set; } = [];
    }
}

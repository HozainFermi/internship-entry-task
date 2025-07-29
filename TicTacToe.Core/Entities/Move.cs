using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;


namespace TicTacToe.Core.Entities
{
    public class Move
    {
        public Guid Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Guid GameId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public char Player { get; set; }
        public string Etag { get; set; } = "no-etag";

        public Game Game { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Entities;
using TicTacToe.Core.Enums;

namespace TicTacToe.Core.Entities
{
    public class Game
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string FirstPlayer { get; set; }
        public string SecondPlayer { get; set; }
        public int BoardSize { get; set; }
        public int WinCon { get; set; }
        public char[][] Board { get; set; }
        public char CurrentPlayer { get; set; } = 'X';
        public GameState State { get; set; } = GameState.InProgress;
        public int MoveCount { get; set; } = 0;
        public string Etag { get; set; } = "no-etag";
        public ICollection<Move> Moves = new List<Move>();

        public Game(int boardSize, string firstplayer, string secondplayer, int wincon )
        {
            BoardSize = boardSize;
            FirstPlayer = firstplayer;
            SecondPlayer = secondplayer;
            WinCon = wincon;
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            Board = new char[BoardSize][];
            for (int i = 0; i < BoardSize; i++)
            {
                Board[i] = new char[BoardSize];
                for (int j = 0; j < BoardSize; j++)
                {
                    Board[i][j] = ' ';
                }
            }
        }

        
    }
}


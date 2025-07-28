using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.Exceptions.Game
{
    public class InvalidMoveException(int row, int column): BadRequestException("Клетка уже занята или находится за пределами игрового поля", new {Row=row, Column=column })
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.Exceptions.Game
{
    public class GameNotFoundException(Guid id) : NotFoundException("Не удалось найти игру.", new { Id = id })
    {
        
    }
}

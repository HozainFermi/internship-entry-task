using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Entities;

namespace TicTacToe.Core.Interfaces.Services
{
    public interface IEtagGenerator
    {
        
            string GetEtag(Move move);
        
    }
}

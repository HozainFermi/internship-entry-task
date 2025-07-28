using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.Exceptions
{
    public class NotFoundException : BaseValuedException
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, object relatedData) : base(message, relatedData) { }
    }
}

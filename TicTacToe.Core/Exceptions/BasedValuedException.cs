using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Core.Interfaces;

namespace TicTacToe.Core.Exceptions
{
    public class BaseValuedException : Exception, IValuedException
    {
        private readonly object? _errorValues;

        protected BaseValuedException(string message, object key) : base(message)
        {
            _errorValues = key;
        }

        protected BaseValuedException(string message) : base(message) { }

        public object? GetErrorValues() => _errorValues;
    }
}

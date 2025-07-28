using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Core.Exceptions
{
    public class BadRequestException : BaseValuedException
    {
        public BadRequestException(string message) : base(message) { }

        public BadRequestException(string message, string details) : base(message)
        {
            Details = details;
        }
        public BadRequestException(string message, object relatedData) : base(message, relatedData) { }

        public string? Details { get; }
    }
}

using System;

namespace TrickingLibrary.Data
{
    public class InvalidVersionException : Exception
    {
        public InvalidVersionException(string message) : base(message){ }
    }
}
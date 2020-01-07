using System;

namespace FlatResults.Exceptions
{
    public class FlatResultsException : Exception
    {
        public FlatResultsException()
        {
        }

        public FlatResultsException(string message) : base(message)
        {
        }

        public FlatResultsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

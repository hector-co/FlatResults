using System;

namespace FlatResults.Exceptions
{
    public class IdPropertyNotFoundException : FlatResultsException
    {
        public IdPropertyNotFoundException()
        {
        }

        public IdPropertyNotFoundException(string message) : base(message)
        {
        }

        public IdPropertyNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

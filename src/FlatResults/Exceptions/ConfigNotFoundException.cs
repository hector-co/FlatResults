namespace FlatResults.Exceptions
{
    public class ConfigNotFoundException : FlatResultsException
    {
        public ConfigNotFoundException()
        {
        }

        public ConfigNotFoundException(string message) : base(message)
        {
        }

        public ConfigNotFoundException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}

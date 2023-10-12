namespace Utilities.Exceptions
{
    public class ValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> ErrorsDictionary { get; set; }

        public ValidationException(IReadOnlyDictionary<string, string[]> errorsDictionary)
        {
            ErrorsDictionary = errorsDictionary;
        }
    }
}

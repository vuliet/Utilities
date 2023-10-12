namespace Utilities
{
    public class BadRequestException : Exception
    {
    }

    public class NotFoundException : Exception
    {
    }

    public class ValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> ErrorsDictionary { get; set; }

        public ValidationException(IReadOnlyDictionary<string, string[]> errorsDictionary)
        {
            ErrorsDictionary = errorsDictionary;
        }
    }

    public class AppException : Exception
    {
        public AppError Error { get; }

        public AppException(
            AppError error = AppError.INVALID_OPERATION,
            string message = "") :
            base(message)
        {
            Error = error;
        }

        public AppException(string message) :
            base(message)
        {
            Error = AppError.INVALID_OPERATION;
        }
    }
}

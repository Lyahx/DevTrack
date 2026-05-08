namespace DevTrack.Domain.Exceptions;

public abstract class AppException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    protected AppException(string code, string message, int statusCode) : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base("NOT_FOUND", message, 404) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base("CONFLICT", message, 409) { }
}

public class ValidationFailedException : AppException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationFailedException(IReadOnlyDictionary<string, string[]> errors)
        : base("VALIDATION_FAILED", "One or more validation errors occurred.", 400)
    {
        Errors = errors;
    }
}

public class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message) : base("UNAUTHORIZED", message, 401) { }
}

public class ForbiddenAppException : AppException
{
    public ForbiddenAppException(string message) : base("FORBIDDEN", message, 403) { }
}

public class AiServiceException : AppException
{
    public AiServiceException(string code, string message, int statusCode = 502) : base(code, message, statusCode) { }
}

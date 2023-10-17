namespace Electronic.Application.Exceptions;

public class ApiException : APIMessageResponse
{
    public ApiException(int statusCode, string message = null, string details = null) : base(statusCode, message)
    {
        Details = details;
    }

    public string Details { get; set; }
}
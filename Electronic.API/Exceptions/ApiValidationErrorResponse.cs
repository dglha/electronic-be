namespace Electronic.Application.Exceptions;

public class ApiValidationErrorResponse : APIMessageResponse
{
    public ApiValidationErrorResponse() : base(400)
    {
    }
    public IEnumerable<string> Errors { get; set; }
}
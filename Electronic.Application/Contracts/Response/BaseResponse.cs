namespace Electronic.Application.Contracts.Response;

public class BaseResponse<T> where T : class
{
    public bool IsSuccess{ get; set; }
    public T Data { get; set; }
    
    public string Message { get; set; }

    public BaseResponse(T data)
    {
        Data = data;
        IsSuccess = true;
        Message = "";
    }

    public BaseResponse()
    {
        
    }
    //
    // public BaseResponse(bool isSuccess, string message, T data = null)
    // {
    //     Data = data;
    //     IsSuccess = true;
    //     Message = message;
    // }
}
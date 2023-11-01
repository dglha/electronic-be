namespace Electronic.Application.Contracts.Response;

public class BaseResponse<T> where T : class
{
    public bool IsSuccess{ get; set; }
    public T Data { get; set; }

    public BaseResponse(T data)
    {
        Data = data;
        IsSuccess = true;
    }
    
    public BaseResponse(bool isSuccess, T data = null)
    {
        Data = data;
        IsSuccess = true;
    }
}
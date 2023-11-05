namespace Electric.Payment.VNPay.DTOs.Response;

public class VnPayIpnResponseDto
{
    public VnPayIpnResponseDto()
    {
                
    }
    public VnPayIpnResponseDto(string rspCode, string message)
    {
        RspCode = rspCode;
        Message = message;
    }
    public void Set(string rspCode, string message)
    {
        RspCode = rspCode;
        Message = message;
    }
    public string RspCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
namespace Electronic.Application.Contracts.DTOs.Payment;

public class PaymentResponseDto
{
    public bool IsSuccess { get; set; }
    public string OrderDescription { get; set; }
    public string OrderId { get; set; }
    public string TransactionId { get; set; }
    public string VnPayResponseCode { get; set; }
    public string TransactionStatus { get; set; }
}
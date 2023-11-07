namespace Electronic.Application.Contracts.DTOs.Payment;

public class PaymentDto
{
    public long PaymentId { get; set; }
    public decimal Amount { get; set; }
    public decimal PaymentFee { get; set; }
    public string GatewayTransactionId { get; set; }
    public string Status { get; set; }
    public string FailureMessage { get; set; }
    public string Customer { get; set; }
}
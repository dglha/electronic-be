using Electronic.Domain.Common;
using Electronic.Domain.Enums;

namespace Electronic.Domain.Models.Payment;

public class Payment : BaseEntity
{
    public long PaymentId { get; set; }
    public long OrderId { get; set; }
    public Order.Order Order { get; set; }
    public decimal Amount { get; set; }
    public decimal PaymentFee { get; set; }
    public string GatewayTransactionId { get; set; }
    public PaymentStatusEnum Status { get; set; }
    public string FailureMessage { get; set; }
    public string CustomerId { get; set; }
}
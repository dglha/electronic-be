namespace Electronic.Domain.Enums;

public enum OrderStatusEnum
{
    New = 1,
    OnHold,
    PendingPayment,
    PaymentReceived,
    PaymentFailed,
    Shipping,
    Shipped,
    Completed,
    Canceled
}
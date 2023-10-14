using Electronic.Domain.Enums;

namespace Electronic.Domain.Models.Order;

public class OrderStatusHistory
{
    public long OrderStatusHistoryId { get; set; }
    public long OrderId { get; set; }
    public Order Order { get; set; }
    public OrderStatusEnum? OldStatus { get; set; }
    public OrderStatusEnum NewStatus { get; set; }
    public string Note { get; set; }
}
using Electronic.Domain.Common;
using Electronic.Domain.Enums;

namespace Electronic.Domain.Models.Order;

public class Order : BaseEntity
{
    public long OrderId { get; set; }
    public OrderStatusEnum OrderStatus { get; set; }
    public string ShippingMethod { get; set; }
    public decimal? ShippingFeeAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal OrderTotal { get; set; }
    public string? PaymentMethod { get; set; }
    public decimal? PaymentFeeAmount { get; set; }

    public ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    
    public string CustomerId { get; set; }
}
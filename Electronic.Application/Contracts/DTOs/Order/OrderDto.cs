namespace Electronic.Application.Contracts.DTOs.Order;

public class OrderDto
{
    public long OrderId { get; set; }
    public string OrderStatus { get; set; }
    public string ShippingMethod { get; set; }
    public decimal ShippingFeeAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal OrderTotal { get; set; }
    public string PaymentMethod { get; set; }
    public decimal PaymentFeeAmount { get; set; }
    public IEnumerable<OrderItemDto> OrderItems { get; set; }
    public IEnumerable<OrderStatusHistoryDto> OrderStatusHistories { get; set; }
    public OrderAddressDto? Address { get; set; }
}
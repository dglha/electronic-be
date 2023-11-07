namespace Electronic.Application.Contracts.DTOs.Order;

public class OrderListDto
{
    public long OrderId { get; set; }
    public string OrderStatus { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal OrderTotal { get; set; }
    public string Customer { get; set; }
}
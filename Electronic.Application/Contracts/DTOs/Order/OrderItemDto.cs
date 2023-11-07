namespace Electronic.Application.Contracts.DTOs.Order;

public class OrderItemDto
{
    public string Name { get; set; }
    public decimal ProductPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public int Quantity { get; set; }
    public string ThumbnailImageUrl { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public List<string> Options { get; set; }
    public string Slug { get; set; }
    public long ProductId { get; set; }
}
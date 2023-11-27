namespace Electronic.Application.Contracts.DTOs.Product;

public class ProductPriceHistoryDto
{
    public long ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public decimal SpecialPrice { get; set; }
    public DateTime SpecialPriceStartDate { get; set; }
    public DateTime SpecialPriceEndDate { get; set; }
}
namespace Electronic.Application.Contracts.DTOs.Stock.Admin;

public class ProductStockDto
{
    public long StockId { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; }
    public long StockQuantity { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
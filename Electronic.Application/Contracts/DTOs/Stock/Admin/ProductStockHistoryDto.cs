namespace Electronic.Application.Contracts.DTOs.Stock.Admin;

public class ProductStockHistoryDto
{
    public long StockHistoryId { get; set; }
    public int OldQuantity { get; set; }
    public int AdjustedQuantity { get; set; }
    public string Note { get; set; }
}
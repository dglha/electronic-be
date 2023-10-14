using Electronic.Domain.Common;

namespace Electronic.Domain.Models.Inventory;

public class StockHistory : BaseEntity
{
    public long StockHistoryId { get; set; }
    public long StockId { get; set; }
    public Stock Stock { get; set; }
    public int OldQuantity { get; set; }
    public int AdjustedQuantity { get; set; }
    public string Note { get; set; }
}
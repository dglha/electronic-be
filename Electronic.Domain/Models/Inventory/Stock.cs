using Electronic.Domain.Common;
using Electronic.Domain.Model.Catalog;

namespace Electronic.Domain.Models.Inventory;

public class Stock : BaseEntity
{
    public long StockId { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
    
    public int Quantity { get; set; }
}
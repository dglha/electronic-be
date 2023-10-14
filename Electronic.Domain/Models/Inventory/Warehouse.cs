using Electronic.Domain.Common;

namespace Electronic.Domain.Models.Inventory;

public class Warehouse : BaseEntity
{
    public int WarehouseId { get; set; }
    public string Address { get; set; }
}
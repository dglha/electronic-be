using Electronic.Domain.Common;
using Electronic.Domain.Model.Catalog;

namespace Electronic.Domain.Models.Catalog;

public class ProductPriceHistory : BaseEntity
{
    public long ProductPriceHistoryId { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public decimal SpecialPrice { get; set; }
    public DateTime SpecialPriceStartDate { get; set; }
    public DateTime SpecialPriceEndDate { get; set; }
}
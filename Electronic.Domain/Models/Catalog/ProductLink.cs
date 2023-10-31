using System.ComponentModel.DataAnnotations.Schema;
using Electronic.Domain.Common;
using Electronic.Domain.Enums;
using Electronic.Domain.Model.Catalog;

namespace Electronic.Domain.Models.Catalog;

public class ProductLink : BaseEntity
{
    public long ProductLinkId { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public long LinkedProductId { get; set; }
    public Product LinkedProduct { get; set; }
    public ProductLinkEnum Type { get; set; }
}
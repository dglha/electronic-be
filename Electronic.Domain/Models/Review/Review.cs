using Electronic.Domain.Common;
using Electronic.Domain.Model.Catalog;

namespace Electronic.Domain.Models.Review;

public class Review : BaseEntity
{
    public long ProductId { get; set; }
    public Product Product { get; set; }
    public long ReviewId { get; set; }
    public int Rating { get; set; }
    public string ReviewerName { get; set; }
    public string ReviewerId { get; set; }
    public string Comment { get; set; }
    public bool Status { get; set; }
}
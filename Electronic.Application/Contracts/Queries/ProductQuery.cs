using Electronic.Application.Contracts.Common;

namespace Electronic.Application.Contracts.Queries;

public class ProductQuery : IQueryObject
{
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsSortAscending { get; set; }
    public string? SortBy { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
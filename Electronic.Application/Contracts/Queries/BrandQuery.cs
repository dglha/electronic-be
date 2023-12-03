using Electronic.Application.Contracts.Common;

namespace Electronic.Application.Contracts.Queries;

public class BrandQuery : IQueryObject
{
    public string Name { get; set; }
    public bool? IsSortAscending { get; set; }
    public string SortBy { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
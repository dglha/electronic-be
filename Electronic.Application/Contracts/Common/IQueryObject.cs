namespace Electronic.Application.Contracts.Common;

public interface IQueryObject
{
    public bool? IsSortAscending { get; set; }

    public string SortBy { get; set; }

    public int? Page { get; set; }

    public int? PageSize { get; set; }
}
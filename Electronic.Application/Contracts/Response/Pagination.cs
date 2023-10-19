namespace Electronic.Application.Contracts.Response;

public class Pagination<T> where T : class
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasNextPage => CurrentPage * PageSize < TotalCount;
    public bool HasPreviousPage => TotalPages > 1;
    public IEnumerable<T> Data { get; set; }


    public Pagination(IEnumerable<T> data, int currentPage, int pageSize, int totalCount)
    {
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
        Data = data;
    }
    
    public static Pagination<T> ToPagination(IEnumerable<T> data, int currentPage, int pageSize, int totalCount)
    {
        return new Pagination<T>(data, currentPage, pageSize, totalCount);
    }
}
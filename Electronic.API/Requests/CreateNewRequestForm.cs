namespace Electronic.API.Requests;

public class CreateNewRequestForm
{
    public string Title { get; set; }
    public string ShortContent { get; set; }
    public string FullContent { get; set; }
    public bool IsPublished { get; set; }
    public bool IsDeleted { get; set; }
    public IFormFile ThumbnailImage { get; set; }
    public IEnumerable<int> NewCatetoryIds { get; set; }
}
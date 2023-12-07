namespace Electronic.API.Requests;

public class CreateAdForm
{
    public string Name { get; set; }
    public int DisplayOrder { get; set; }
    public string Description { get; set; }
    public IFormFile Image { get; set; }
}
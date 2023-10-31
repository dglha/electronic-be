namespace Electronic.API.Requests;

public class TestIFormFile
{
    public IFormFile Image { get; set; }
    public string Name { get; set; }
    public IEnumerable<TestChild> Children { get; set; }
}
namespace Electronic.API.Requests;

public class UpdateProductRequestForm : CreateProductRequestForm
{
    public IList<long> DeletedMediaIds { get; set; } = new List<long>();
}
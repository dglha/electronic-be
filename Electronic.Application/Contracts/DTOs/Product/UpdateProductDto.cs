namespace Electronic.Application.Contracts.DTOs.Product;

public class UpdateProductDto : CreateProductDto
{
    public IList<long> DeletedMediaIds { get; set; } = new List<long>();
}
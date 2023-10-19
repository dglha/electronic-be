namespace Electronic.Application.Contracts.DTOs.Brand;

public class BrandDto : CreateBrandResultDto
{
    public int BrandId { get; set; }
    public bool IsPublished { get; set; }
}
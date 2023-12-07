using System.ComponentModel.DataAnnotations;
using Electronic.Application.Contracts.Common;

namespace Electronic.Application.Contracts.DTOs.Advertisement;

public class CreateAdvertisementDto
{
    [Required]
    [Range(1, 3)]
    public int DisplayOrder { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ImageFileDto Image { get; set; }
}
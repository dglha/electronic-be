using System.ComponentModel.DataAnnotations;
using Electronic.Domain.Common;
using Electronic.Domain.Enums;

namespace Electronic.Domain.Models.Core;

public class Media : BaseEntity
{
    public long MediaId { get; set; }
    public string Caption { get; set; }
    public int FileSize { get; set; }
    public string FileName { get; set; }
    public MediaTypeEnum MediaType { get; set; }
}
using Electronic.Domain.Common;
using Electronic.Domain.Models.Core;

namespace Electronic.Domain.Models;

public class Advertisement : BaseEntity
{
    public Advertisement() {}
    
    public int AdvertisementId { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public int DisplayOrder { get; set; }
    
    public int SortIndex { get; set; }
    
    public bool IsPublished { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public long MediaId { get; set; }
    
    public Media Media { get; set; }
}
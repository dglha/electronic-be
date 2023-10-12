using System.ComponentModel.DataAnnotations.Schema;
using Electronic.Domain.Models.Core;

namespace Electronic.Domain.Models.New;

public class NewItem
{
    public NewItem()
    {
        
    }
    
    public int NewItemId { get; set; }

    public string Title { get; set; }
    
    public string Slug { get; set; }
    
    public string ShortContent { get; set; }
    
    public string FullContent { get; set; }
    
    public bool IsPublished { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime PublishedAt { get; set; }
    
    public long? ThumbnailImageId { get; set; }
    
    public Media? ThumbnailImage { get; set; }

    public ICollection<NewItemNewCategory> NewItemNewCategories { get; set; } = new List<NewItemNewCategory>();
}
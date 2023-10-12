namespace Electronic.Domain.Models.New;

public class NewItemNewCategory
{
    public int NewItemId { get; set; }
    public NewItem NewItem { get; set; }
    public int NewCategoryId { get; set; }
    public NewCategory NewCategory { get; set; }
}
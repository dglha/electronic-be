using Electronic.Domain.Common;

namespace Electronic.Domain.Models.Core;

public class Country : BaseEntity
{
    public Country()
    {
    }
    
    public int CountryId { get; set; }

    public string Name { get; set; }

    public string Code3 { get; set; }

    public ICollection<StateOrProvince> StateOrProvinces { get; set; } = new List<StateOrProvince>();
}
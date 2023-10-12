using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Electronic.Domain.Models.Core;

public class StateOrProvince
{
    public StateOrProvince() {}
    
    public int StateOrProvinceId { get; set; }
    
    public int CountryId { get; set; }
    
    public Country Country { get; set; }
    
    public string Code { get; set; }
    
    public string Name { get; set; }
    
    public string Type { get; set; }

    public ICollection<District> Districts { get; set; } = new List<District>();
}
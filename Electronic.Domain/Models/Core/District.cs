using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Electronic.Domain.Models.Core;

public class District
{
    public District() {}
    
    public int DistrictId { get; set; }
    
    public int StateOrProvinceId { get; set; }
    
    public StateOrProvince StateOrProvince { get; set; }
    
    public string Name { get; set; }
    
    public string Type { get; set; }
    
    public string Location { get; set; }
}
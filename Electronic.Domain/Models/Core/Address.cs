using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Electronic.Domain.Common;

namespace Electronic.Domain.Models.Core;

public class Address : BaseEntity
{
    public Address() {}
    
    public int AddressId { get; set; }
    
    public string ContactName { get; set; }
    
    public string AddressLineOne { get; set; }
    
    public string AddressLineTwo { get; set; }
    
    public string City { get; set; }

    public string ZipCode { get; set; }
    
    public int? DistrictId { get; set; }
    
    public District? District { get; set; }
    
    public int? CountryId { get; set; }
    
    public Country? Country { get; set; }
    
    public int? StateOrProvinceId { get; set; }
    
    public StateOrProvince? StateOrProvince { get; set; }
    
    public string CustomerId { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string Email { get; set; }

    public bool IsDefault { get; set; } = false;
}
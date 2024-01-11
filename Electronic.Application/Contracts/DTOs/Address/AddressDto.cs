namespace Electronic.Application.Contracts.DTOs.Address;

public class AddressDto
{
    public int AddressId { get; set; }
    public string UserId { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string Zipcode { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
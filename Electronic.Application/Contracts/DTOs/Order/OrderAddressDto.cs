namespace Electronic.Application.Contracts.DTOs.Order;

public class OrderAddressDto
{
    public long OrderId { get; set; }
    public string FirstName { get; set; }
    public string Lastname { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public string ZipCode { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string PaymentMethod { get; set; }
}
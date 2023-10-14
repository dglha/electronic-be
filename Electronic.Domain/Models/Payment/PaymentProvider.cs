using Electronic.Domain.Common;

namespace Electronic.Domain.Models.Payment;

public class PaymentProvider : BaseEntity
{
    public int PaymentProviderId { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }
    public string ConfigureUrl { get; set; }
    public string AdditionalSettings { get; set; }
}
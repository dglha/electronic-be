namespace Electric.Payment.VNPay.Config;

public class VnPayConfig
{
    // public static string ConfigName => "Vnpay";
    public string Version { get; set; }
    public string TmnCode { get; set; } 
    public string HashSecret { get; set; }
    public string ReturnUrl { get; set; }
    public string PaymentUrl { get; set;} 
}
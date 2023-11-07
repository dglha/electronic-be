namespace Electronic.Application.Contracts.DTOs.Order;

public class OrderStatusHistoryDto
{
    public string? OldStatus { get; set; }
    public string NewStatus { get; set; }
    public string Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
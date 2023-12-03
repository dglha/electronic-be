namespace Electronic.Application.Contracts.DTOs.Stock.Admin;

public class UpdateProductStockRequestDto
{
    public long ProductId { get; set; }
    public int AdjustedAmount { get; set; }
    public string Note { get; set; }
}
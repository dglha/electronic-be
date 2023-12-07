using Electronic.Application.Contracts.DTOs.Product;

namespace Electronic.Application.Contracts.DTOs.Dashboard;

public class DashboardDto
{
    public int TodayOrder { get; set; }
    public int TodayCompletedOrder { get; set; }
    public decimal TotalIncome { get; set; }
    public int TotalSold { get; set; }
    public List<ProductListDto> TopSoldProduct { get; set; }
}
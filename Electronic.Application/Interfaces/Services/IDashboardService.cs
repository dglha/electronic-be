using Electronic.Application.Contracts.DTOs.Dashboard;
using Electronic.Application.Contracts.DTOs.Payment;
using Electronic.Application.Contracts.Response;

namespace Electronic.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<BaseResponse<DashboardDto>> GetDashboarData();
    Task<Pagination<PaymentDto>> GetLatestPayment();
}
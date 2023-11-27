using Electronic.Application.Contracts.DTOs.Stock.Admin;
using Electronic.Application.Contracts.Response;

namespace Electronic.Application.Interfaces.Services;

public interface IStockService
{
    public Task UpdateProductStockQuantity(UpdateProductStockRequestDto request);

    public Task<Pagination<ProductStockDto>> GetListStock(string? productName, int currentPage, int itemPerPage);

    public Task<Pagination<ProductStockHistoryDto>> GetProductStockHistory(long productId, int currentPage,
        int itemPerPage);
}
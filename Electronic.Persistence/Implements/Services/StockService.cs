using System.Net;
using Electronic.Application.Contracts.DTOs.Stock.Admin;
using Electronic.Application.Contracts.Exeptions;
using Electronic.Application.Contracts.Logging;
using Electronic.Application.Contracts.Response;
using Electronic.Application.Interfaces.Services;
using Electronic.Domain.Enums;
using Electronic.Domain.Model.Catalog;
using Electronic.Domain.Models.Inventory;
using Electronic.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Implements.Services;

public class StockService : IStockService
{
    private readonly ElectronicDatabaseContext _dbContext;
    private readonly IAppLogger<StockService> _logger;

    public StockService(ElectronicDatabaseContext dbContext, IAppLogger<StockService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task UpdateProductStockQuantity(UpdateProductStockRequestDto request)
    {
        if (request.AdjustedAmount == 0)
            throw new AppException("Invalid adjustment, try another number than 0!", (int)HttpStatusCode.BadRequest);

        var product = await _dbContext.Set<Product>().Where(p => p.ProductId == request.ProductId)
            .FirstOrDefaultAsync();

        if (product == null) throw new AppException("Product not found", (int)HttpStatusCode.BadRequest);

        if (product.HasOption)
            throw new AppException("Please adjust product's variants!", (int)HttpStatusCode.BadRequest);

        if (product.StockQuantity is 0 && request.AdjustedAmount <= 0)
            throw new AppException("Invalid adjustment, try again!", (int)HttpStatusCode.BadRequest);

        if (product.StockQuantity.HasValue && product.StockQuantity.Value + request.AdjustedAmount < 0)
            throw new AppException("Invalid input, try again!", (int)HttpStatusCode.BadRequest);

        var productStock = await _dbContext.Set<Stock>().Where(p => p.ProductId == product.ProductId)
            .FirstOrDefaultAsync();

        if (productStock == null)
        {
            var warehouse = _dbContext.Set<Warehouse>().First();
            // Create new Stock to tracking product Stock
            productStock = new Stock
            {
                ProductId = product.ProductId,
                Quantity = product.StockQuantity ?? 0,
                Warehouse = warehouse // Default, implement later
            };

            await _dbContext.Set<Stock>().AddAsync(productStock);
        }

        var oldQuantity = product.StockQuantity ?? 0;

        product.StockQuantity = product.StockQuantity != null
            ? product.StockQuantity.Value + request.AdjustedAmount
            : request.AdjustedAmount;

        productStock.Quantity = (int)product.StockQuantity;

        // Create product stock history
        var productStockHistory = new StockHistory
        {
            Stock = productStock,
            OldQuantity = oldQuantity,
            AdjustedQuantity = request.AdjustedAmount,
            Note = string.IsNullOrEmpty(request.Note) ? StockHistoryNoteEnum.Adjustment.ToString() :request.Note
        };

        await _dbContext.Set<StockHistory>().AddAsync(productStockHistory);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Pagination<ProductStockDto>> GetListStock(string? productName, int currentPage, int itemPerPage)
    {
        var productQuery = _dbContext.Set<Product>().Where(p => !p.IsDeleted && !p.HasOption);
        if (!string.IsNullOrEmpty(productName))
        {
            productQuery = productQuery.Where(p => p.Name.Contains(productName));
        }

        var validProductIds = await productQuery.Select(p => p.ProductId).ToListAsync();

        var stockQuery = _dbContext.Set<Stock>().Include(s => s.Product)
            .Where(s => validProductIds.Contains(s.ProductId));

        var totalCount = await stockQuery.CountAsync();

        if (currentPage == -1)
        {
            itemPerPage = totalCount;
            currentPage *= -1;
        }

        var data = await stockQuery.Skip((currentPage - 1) * itemPerPage).Take(itemPerPage).Select(s =>
            new ProductStockDto
            {
                StockId = s.StockId,
                ProductId = s.ProductId,
                StockQuantity = s.Quantity,
                ProductName = s.Product.Name,
                UpdatedAt = s.UpdatedAt,
            }).ToListAsync();
        return Pagination<ProductStockDto>.ToPagination(data, currentPage, itemPerPage, totalCount);
    }

    public async Task<Pagination<ProductStockHistoryDto>> GetProductStockHistory(long productId, int currentPage,
        int itemPerPage)
    {
        var stock = await _dbContext.Set<Stock>().Where(s => s.ProductId == productId).FirstOrDefaultAsync();
        if (stock is null)
        {
            return Pagination<ProductStockHistoryDto>.ToPagination(new List<ProductStockHistoryDto>(), currentPage,
                itemPerPage, 0);
        }

        var query = _dbContext.Set<StockHistory>().Where(s => s.StockId == stock.StockId);
        var totalCount = await query.CountAsync();
        if (currentPage == -1)
        {
            itemPerPage = totalCount;
            currentPage *= -1;
        }

        var data = await query.Skip((currentPage - 1) * itemPerPage).Take(itemPerPage).Select(s =>
            new ProductStockHistoryDto
            {
                Note = s.Note,
                AdjustedQuantity = s.AdjustedQuantity,
                OldQuantity = s.OldQuantity,
                StockHistoryId = s.StockHistoryId
            }).ToListAsync();
        return Pagination<ProductStockHistoryDto>.ToPagination(data, currentPage, itemPerPage, totalCount);
    }
}
using Electronic.Application.Contracts.Common;
using Electronic.Application.Contracts.DTOs.Advertisement;

namespace Electronic.Application.Interfaces.Services;

public interface IAdvertisementService
{
    public Task<List<AdvertisementDto>> GetAd();
    public Task AddNewAdvertisement(CreateAdvertisementDto request);
}
using Electronic.Application.Contracts.Persistences;
using Electronic.Domain.Common;
using Electronic.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace Electronic.Persistence.Interfaces.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly ElectronicDatabaseContext _dbContext;

    public GenericRepository(ElectronicDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await  _dbContext.Set<T>().ToListAsync();
    }

    public async Task<T?> GetAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Set<T>().Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();

    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}
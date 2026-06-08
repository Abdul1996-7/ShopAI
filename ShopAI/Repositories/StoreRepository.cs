using Microsoft.EntityFrameworkCore;
using ShopAI.Data;
using ShopAI.Models;

namespace ShopAI.Repositories;

public sealed class StoreRepository(ApplicationDbContext db, ILogger<StoreRepository> logger) : IStoreRepository
{
    public async Task<Store?> GetByIdAsync(int id)
    {
        return await db.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(store => store.Id == id);
    }

    public async Task<Store?> GetBySlugAsync(string slug)
    {
        return await db.Stores
            .AsNoTracking()
            .FirstOrDefaultAsync(store => store.Slug == slug && store.IsActive);
    }

    public async Task<Store> CreateAsync(Store store)
    {
        try
        {
            store.CreatedAt = DateTime.UtcNow;
            db.Stores.Add(store);
            await db.SaveChangesAsync();
            return store;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to create store {StoreName}.", store.Name);
            throw;
        }
    }

    public async Task<Store> UpdateAsync(Store store)
    {
        try
        {
            db.Stores.Update(store);
            await db.SaveChangesAsync();
            return store;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to update store {StoreId}.", store.Id);
            throw;
        }
    }
}

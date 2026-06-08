using Microsoft.EntityFrameworkCore;
using ShopAI.Data;
using ShopAI.Models;

namespace ShopAI.Repositories;

public sealed class ProductRepository(ApplicationDbContext db, ILogger<ProductRepository> logger) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetFeaturedAsync(int count = 8)
    {
        return await PublicProducts()
            .OrderByDescending(product => product.ViewCount)
            .ThenByDescending(product => product.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await db.Products
            .AsNoTracking()
            .Include(product => product.Store)
            .Include(product => product.Category)
            .FirstOrDefaultAsync(product => product.Id == id);
    }

    public async Task<IReadOnlyList<Product>> GetByStoreAsync(int storeId, int page, int pageSize)
    {
        var skip = Math.Max(page - 1, 0) * pageSize;

        return await PublicProducts()
            .Where(product => product.StoreId == storeId)
            .OrderByDescending(product => product.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(string query, int? categoryId)
    {
        query = (query ?? string.Empty).Trim();

        var products = PublicProducts();

        if (categoryId is not null)
        {
            products = products.Where(product => product.CategoryId == categoryId);
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            products = products.Where(product =>
                product.Title.Contains(query) ||
                product.Description.Contains(query) ||
                product.Category.Name.Contains(query) ||
                product.Store.Name.Contains(query));
        }

        return await products
            .OrderByDescending(product => product.UpdatedAt)
            .ThenByDescending(product => product.CreatedAt)
            .ToListAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        try
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return product;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to create product {ProductTitle}.", product.Title);
            throw;
        }
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        try
        {
            product.UpdatedAt = DateTime.UtcNow;
            db.Products.Update(product);
            await db.SaveChangesAsync();
            return product;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to update product {ProductId}.", product.Id);
            throw;
        }
    }

    public async Task IncrementViewCountAsync(int productId)
    {
        try
        {
            await db.Products
                .Where(product => product.Id == productId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(product => product.ViewCount, product => product.ViewCount + 1));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to increment view count for product {ProductId}.", productId);
            throw;
        }
    }

    private IQueryable<Product> PublicProducts()
    {
        return db.Products
            .AsNoTracking()
            .Include(product => product.Store)
            .Include(product => product.Category)
            .Where(product => product.IsPublished && product.IsAvailable && product.Store.IsActive);
    }
}

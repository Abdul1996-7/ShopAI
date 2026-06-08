using Microsoft.EntityFrameworkCore;
using ShopAI.Data;
using ShopAI.Models;

namespace ShopAI.Repositories;

public sealed class CategoryRepository(ApplicationDbContext db) : ICategoryRepository
{
    public async Task<IReadOnlyList<Category>> GetAllAsync()
    {
        return await db.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(category => category.Slug == slug);
    }
}

using ShopAI.Models;

namespace ShopAI.Repositories;

public interface ICategoryRepository
{
    /// <summary>
    /// Gets all categories ordered by display name.
    /// </summary>
    Task<IReadOnlyList<Category>> GetAllAsync();

    /// <summary>
    /// Gets a category by its public slug.
    /// </summary>
    Task<Category?> GetBySlugAsync(string slug);
}

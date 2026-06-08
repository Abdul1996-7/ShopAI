using ShopAI.Models;

namespace ShopAI.Repositories;

public interface IStoreRepository
{
    /// <summary>
    /// Gets a store by its database identifier.
    /// </summary>
    Task<Store?> GetByIdAsync(int id);

    /// <summary>
    /// Gets a store by its public slug.
    /// </summary>
    Task<Store?> GetBySlugAsync(string slug);

    /// <summary>
    /// Creates a store and persists it to the database.
    /// </summary>
    Task<Store> CreateAsync(Store store);

    /// <summary>
    /// Updates a store and persists the changes to the database.
    /// </summary>
    Task<Store> UpdateAsync(Store store);
}

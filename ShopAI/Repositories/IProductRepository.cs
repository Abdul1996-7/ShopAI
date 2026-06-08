using ShopAI.Models;

namespace ShopAI.Repositories;

public interface IProductRepository
{
    /// <summary>
    /// Gets a limited set of published products for the public homepage.
    /// </summary>
    Task<IReadOnlyList<Product>> GetFeaturedAsync(int count = 8);

    /// <summary>
    /// Gets a single product by identifier with its store and category loaded.
    /// </summary>
    Task<Product?> GetByIdAsync(int id);

    /// <summary>
    /// Gets a paged list of published products belonging to a store.
    /// </summary>
    Task<IReadOnlyList<Product>> GetByStoreAsync(int storeId, int page, int pageSize);

    /// <summary>
    /// Searches published products by text and an optional category identifier.
    /// </summary>
    Task<IReadOnlyList<Product>> SearchAsync(string query, int? categoryId);

    /// <summary>
    /// Creates a product and persists it to the database.
    /// </summary>
    Task<Product> CreateAsync(Product product);

    /// <summary>
    /// Updates a product and persists the changes to the database.
    /// </summary>
    Task<Product> UpdateAsync(Product product);

    /// <summary>
    /// Increments the public view counter for a product.
    /// </summary>
    Task IncrementViewCountAsync(int productId);
}

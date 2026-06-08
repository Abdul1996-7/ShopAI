namespace ShopAI.ViewModels;

public sealed record ProductDetailViewModel(
    int Id,
    string Title,
    string Slug,
    string Description,
    decimal Price,
    int InventoryCount,
    bool IsAvailable,
    IReadOnlyList<string> ImageUrls,
    IReadOnlyDictionary<string, string> Specifications,
    string Condition,
    string CategoryName,
    string StoreName,
    string StoreSlug,
    int ViewCount,
    IReadOnlyList<ProductCardViewModel> RelatedProducts);

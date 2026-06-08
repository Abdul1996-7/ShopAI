namespace ShopAI.ViewModels;

public sealed record ProductCardViewModel(
    int Id,
    string Title,
    string Slug,
    decimal Price,
    string PrimaryImageUrl,
    string CategoryName,
    string Condition,
    string StoreSlug,
    string StoreName);

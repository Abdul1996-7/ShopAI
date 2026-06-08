namespace ShopAI.ViewModels;

public sealed record StorePageViewModel(
    int StoreId,
    string StoreName,
    string StoreSlug,
    string? LogoUrl,
    string Description,
    IReadOnlyList<ProductCardViewModel> Products,
    int CurrentPage,
    int TotalPages);

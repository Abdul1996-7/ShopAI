namespace ShopAI.ViewModels;

public sealed record SearchResultsViewModel(
    string Query,
    int? CategoryId,
    IReadOnlyList<CategoryPillViewModel> Categories,
    IReadOnlyList<ProductCardViewModel> Products,
    int CurrentPage,
    int TotalPages,
    int TotalResults);

namespace ShopAI.ViewModels;

public sealed record ProductCarouselSectionViewModel(
    string Id,
    string Title,
    string Eyebrow,
    string ViewAllUrl,
    IReadOnlyList<ProductCardViewModel> Products);

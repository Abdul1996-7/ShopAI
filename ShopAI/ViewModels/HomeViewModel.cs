namespace ShopAI.ViewModels;

public sealed record HomeViewModel(
    IReadOnlyList<ProductCardViewModel> FeaturedProducts,
    IReadOnlyList<CategoryPillViewModel> Categories,
    string HeroText);

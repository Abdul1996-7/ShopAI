namespace ShopAI.ViewModels;

public sealed record DashboardProductRowViewModel(
    int Id,
    string Title,
    string CategoryName,
    decimal Price,
    int InventoryCount,
    bool IsPublished,
    bool IsAvailable,
    int ViewCount,
    string PrimaryImageUrl);

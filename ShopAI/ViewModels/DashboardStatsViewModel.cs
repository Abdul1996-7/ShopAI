namespace ShopAI.ViewModels;

public sealed record DashboardStatsViewModel(
    int TotalProducts,
    int TotalViews,
    int LowInventoryProducts,
    int RecentOrders,
    int InStockProducts,
    int OutOfStockProducts,
    IReadOnlyList<DashboardProductRowViewModel> RecentProducts);

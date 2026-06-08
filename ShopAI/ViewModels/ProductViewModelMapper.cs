using ShopAI.Helpers;
using ShopAI.Models;

namespace ShopAI.ViewModels;

public static class ProductViewModelMapper
{
    public static ProductCardViewModel ToCard(Product product)
    {
        var primaryImage = product.ImageUrls.FirstOrDefault() ?? "/images/product-placeholder.svg";

        return new ProductCardViewModel(
            product.Id,
            product.Title,
            SlugHelper.GenerateSlug(product.Title),
            product.Price,
            ToThumbnailUrl(primaryImage),
            FormatCategoryName(product.Category.Name),
            FormatCondition(product.Condition),
            product.Store.Slug,
            product.Store.Name);
    }

    public static DashboardProductRowViewModel ToDashboardRow(Product product)
    {
        var primaryImage = product.ImageUrls.FirstOrDefault() ?? "/images/product-placeholder.svg";

        return new DashboardProductRowViewModel(
            product.Id,
            product.Title,
            FormatCategoryName(product.Category.Name),
            product.Price,
            product.InventoryCount,
            product.IsPublished,
            product.IsAvailable,
            product.ViewCount,
            ToThumbnailUrl(primaryImage));
    }

    public static string FormatCondition(ProductCondition condition)
    {
        return condition switch
        {
            ProductCondition.LikeNew => "شبه جديد",
            ProductCondition.Good => "جيد",
            ProductCondition.Fair => "مقبول",
            _ => "جديد"
        };
    }

    public static string FormatCategoryName(string categoryName)
    {
        return categoryName.Trim().ToLowerInvariant() switch
        {
            "electronics" => "الإلكترونيات",
            "mobile phones" => "الهواتف",
            "laptops" => "اللابتوبات",
            "accessories" => "الإكسسوارات",
            "gaming" => "الألعاب",
            _ => categoryName
        };
    }

    private static string ToThumbnailUrl(string imageUrl)
    {
        if (!imageUrl.StartsWith("/uploads/products/", StringComparison.OrdinalIgnoreCase) ||
            !imageUrl.EndsWith(".webp", StringComparison.OrdinalIgnoreCase))
        {
            return imageUrl;
        }

        return imageUrl[..^5] + "_thumb.webp";
    }
}

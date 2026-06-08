using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShopAI.Models;
using ShopAI.Repositories;
using ShopAI.ViewModels;

namespace ShopAI.Controllers;

public sealed class HomeController(IProductRepository productRepository, ICategoryRepository categoryRepository)
    : Controller
{
    private const int SearchPageSize = 12;
    private const int ProductSectionSize = 12;

    public async Task<IActionResult> Index()
    {
        var featuredProducts = await productRepository.GetFeaturedAsync();
        var categories = await categoryRepository.GetAllAsync();

        var model = new HomeViewModel(
            featuredProducts.Select(ProductViewModelMapper.ToCard).ToList(),
            ToCategoryPills(categories),
            "واجهات بيع فاخرة لتجار الإلكترونيات.");

        return View(model);
    }

    [HttpGet("/search")]
    public async Task<IActionResult> Search(string? q, int? category, int page = 1)
    {
        return View(await BuildSearchResultsModelAsync(q, category, page));
    }

    [HttpGet("/search/results")]
    public async Task<IActionResult> SearchResults(string? q, int? category, int page = 1)
    {
        return PartialView("_SearchResults", await BuildSearchResultsModelAsync(q, category, page));
    }

    [HttpGet("/partials/products/{sectionKey}")]
    public async Task<IActionResult> ProductSection(string sectionKey)
    {
        return PartialView("_ProductCarousel", await BuildProductSectionAsync(sectionKey));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<SearchResultsViewModel> BuildSearchResultsModelAsync(string? q, int? category, int page)
    {
        page = Math.Max(1, page);

        var products = await productRepository.SearchAsync(q ?? string.Empty, category);
        var categories = await categoryRepository.GetAllAsync();
        var totalResults = products.Count;
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalResults / (double)SearchPageSize));
        var currentPage = Math.Min(page, totalPages);

        var model = new SearchResultsViewModel(
            q ?? string.Empty,
            category,
            ToCategoryPills(categories),
            products
                .Skip((currentPage - 1) * SearchPageSize)
                .Take(SearchPageSize)
                .Select(ProductViewModelMapper.ToCard)
                .ToList(),
            currentPage,
            totalPages,
            totalResults);

        return model;
    }

    private async Task<ProductCarouselSectionViewModel> BuildProductSectionAsync(string sectionKey)
    {
        var categories = await categoryRepository.GetAllAsync();
        var normalizedSection = sectionKey.Trim().ToLowerInvariant();
        IReadOnlyList<Product> products;
        var title = "وصل حديثاً";
        var eyebrow = "اختيارات SmartStore";
        var viewAllUrl = "/search";

        if (normalizedSection == "best-sellers")
        {
            title = "الأكثر مبيعاً";
            eyebrow = "الأعلى مشاهدة بين العملاء";
            products = await productRepository.GetFeaturedAsync(ProductSectionSize);
        }
        else if (normalizedSection == "new-arrivals")
        {
            title = "وصل حديثاً";
            eyebrow = "أحدث المنتجات المنشورة";
            products = (await productRepository.SearchAsync(string.Empty, null)).Take(ProductSectionSize).ToList();
        }
        else
        {
            var categorySlug = normalizedSection switch
            {
                "laptops" => "laptops",
                "mobiles" => "mobile-phones",
                "gaming" => "gaming",
                "accessories" => "accessories",
                _ => "electronics"
            };
            var category = categories.FirstOrDefault(item => item.Slug.Equals(categorySlug, StringComparison.OrdinalIgnoreCase));
            var categoryId = category?.Id;
            title = ProductViewModelMapper.FormatCategoryName(category?.Name ?? "Electronics");
            eyebrow = "منتجات مختارة من القسم";
            viewAllUrl = categoryId is null ? "/search" : $"/search?category={categoryId}";
            products = categoryId is null
                ? []
                : (await productRepository.SearchAsync(string.Empty, categoryId)).Take(ProductSectionSize).ToList();
        }

        return new ProductCarouselSectionViewModel(
            normalizedSection,
            title,
            eyebrow,
            viewAllUrl,
            products.Select(ProductViewModelMapper.ToCard).ToList());
    }

    private static IReadOnlyList<CategoryPillViewModel> ToCategoryPills(IReadOnlyList<Category> categories)
    {
        return categories
            .Select(category => new CategoryPillViewModel(
                category.Id,
                ProductViewModelMapper.FormatCategoryName(category.Name),
                category.Slug,
                category.Description))
            .ToList();
    }
}

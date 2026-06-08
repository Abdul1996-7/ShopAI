using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAI.Data;
using ShopAI.Repositories;
using ShopAI.ViewModels;

namespace ShopAI.Controllers;

public sealed class ProductController(
    IProductRepository productRepository,
    IStoreRepository storeRepository,
    ApplicationDbContext db) : Controller
{
    private const int StorePageSize = 12;

    [HttpGet("/product/{id:int}/{slug?}")]
    public async Task<IActionResult> Detail(int id, string? slug)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null || !product.IsPublished || !product.IsAvailable || !product.Store.IsActive)
        {
            return NotFound();
        }

        await productRepository.IncrementViewCountAsync(id);

        var relatedProducts = (await productRepository.SearchAsync(string.Empty, product.CategoryId))
            .Where(item => item.Id != product.Id)
            .Take(4)
            .Select(ProductViewModelMapper.ToCard)
            .ToList();

        var model = new ProductDetailViewModel(
            product.Id,
            product.Title,
            ProductViewModelMapper.ToCard(product).Slug,
            product.Description,
            product.Price,
            product.InventoryCount,
            product.IsAvailable,
            product.ImageUrls.Count > 0 ? product.ImageUrls : ["/images/product-placeholder.svg"],
            ParseSpecifications(product.SpecificationsJson),
            ProductViewModelMapper.FormatCondition(product.Condition),
            ProductViewModelMapper.FormatCategoryName(product.Category.Name),
            product.Store.Name,
            product.Store.Slug,
            product.ViewCount + 1,
            relatedProducts);

        return View(model);
    }

    [HttpGet("/store/{storeSlug}")]
    public async Task<IActionResult> Store(string storeSlug, int page = 1)
    {
        page = Math.Max(1, page);

        var store = await storeRepository.GetBySlugAsync(storeSlug);
        if (store is null)
        {
            return NotFound();
        }

        var totalProducts = await db.Products
            .AsNoTracking()
            .CountAsync(product => product.StoreId == store.Id && product.IsPublished && product.IsAvailable);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalProducts / (double)StorePageSize));
        var currentPage = Math.Min(page, totalPages);
        var products = await productRepository.GetByStoreAsync(store.Id, currentPage, StorePageSize);

        var model = new StorePageViewModel(
            store.Id,
            store.Name,
            store.Slug,
            store.LogoUrl,
            store.Description,
            products.Select(ProductViewModelMapper.ToCard).ToList(),
            currentPage,
            totalPages);

        return View(model);
    }

    private static IReadOnlyDictionary<string, string> ParseSpecifications(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new Dictionary<string, string>();
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }
        catch (JsonException)
        {
            return new Dictionary<string, string>();
        }
    }
}

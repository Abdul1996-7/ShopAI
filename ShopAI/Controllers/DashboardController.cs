using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopAI.Data;
using ShopAI.Models;
using ShopAI.Repositories;
using ShopAI.Services;
using ShopAI.ViewModels;

namespace ShopAI.Controllers;

[Authorize]
[Route("dashboard")]
public sealed class DashboardController(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext db,
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IImageService imageService,
    ILogger<DashboardController> logger) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var storeId = await GetCurrentStoreIdAsync();
        if (storeId is null)
        {
            return RedirectToAction(nameof(AccountController.Register), "Account");
        }

        var products = await db.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Include(product => product.Store)
            .Where(product => product.StoreId == storeId.Value)
            .OrderByDescending(product => product.UpdatedAt)
            .ToListAsync();

        var recentOrderCutoff = DateTime.UtcNow.AddDays(-30);
        var recentOrders = await db.Orders.CountAsync(order => order.StoreId == storeId.Value && order.CreatedAt >= recentOrderCutoff);

        var model = new DashboardStatsViewModel(
            products.Count,
            products.Sum(product => product.ViewCount),
            products.Count(product => product.InventoryCount <= 3 && product.IsPublished),
            recentOrders,
            products.Count(product => product.InventoryCount > 0 && product.IsAvailable),
            products.Count(product => product.InventoryCount == 0 || !product.IsAvailable),
            products.Take(6).Select(ProductViewModelMapper.ToDashboardRow).ToList());

        return View(model);
    }

    [HttpGet("products")]
    public async Task<IActionResult> Products()
    {
        var storeId = await GetCurrentStoreIdAsync();
        if (storeId is null)
        {
            return RedirectToAction(nameof(AccountController.Register), "Account");
        }

        var products = await db.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Include(product => product.Store)
            .Where(product => product.StoreId == storeId.Value)
            .OrderByDescending(product => product.UpdatedAt)
            .ToListAsync();

        return View(new DashboardProductsViewModel(products.Select(ProductViewModelMapper.ToDashboardRow).ToList()));
    }

    [HttpGet("products/create")]
    public async Task<IActionResult> Create()
    {
        return View("CreateProduct", await BuildProductFormModelAsync(new CreateProductViewModel()));
    }

    [HttpPost("products/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        var storeId = await GetCurrentStoreIdAsync();
        if (storeId is null)
        {
            return RedirectToAction(nameof(AccountController.Register), "Account");
        }

        if (!ModelState.IsValid)
        {
            return View("CreateProduct", await BuildProductFormModelAsync(model));
        }

        try
        {
            var imageUrls = new List<string>();
            foreach (var file in model.ImageFiles.Where(file => file.Length > 0))
            {
                imageUrls.Add(await imageService.UploadProductImageAsync(file, storeId.Value));
            }

            var product = new Product
            {
                StoreId = storeId.Value,
                CategoryId = model.CategoryId,
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                MinimumNegotiablePrice = model.MinimumNegotiablePrice,
                InventoryCount = model.InventoryCount,
                IsPublished = model.IsPublished,
                IsAvailable = model.IsAvailable,
                Condition = model.Condition,
                ImageUrls = imageUrls,
                SpecificationsJson = BuildSpecificationsJson(model)
            };

            await productRepository.CreateAsync(product);
            TempData["StatusMessage"] = "Product created.";

            return RedirectToAction(nameof(Products));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to create product for store {StoreId}.", storeId.Value);
            ModelState.AddModelError(string.Empty, "The product could not be saved. Please try again.");
            return View("CreateProduct", await BuildProductFormModelAsync(model));
        }
    }

    [HttpGet("products/edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var storeId = await GetCurrentStoreIdAsync();
        if (storeId is null)
        {
            return RedirectToAction(nameof(AccountController.Register), "Account");
        }

        var product = await db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id && item.StoreId == storeId.Value);

        if (product is null)
        {
            return NotFound();
        }

        var specifications = ParseSpecifications(product.SpecificationsJson);
        var model = new CreateProductViewModel
        {
            Id = product.Id,
            Title = product.Title,
            Description = product.Description,
            CategoryId = product.CategoryId,
            Price = product.Price,
            MinimumNegotiablePrice = product.MinimumNegotiablePrice,
            InventoryCount = product.InventoryCount,
            IsPublished = product.IsPublished,
            IsAvailable = product.IsAvailable,
            Condition = product.Condition,
            ExistingImageUrls = product.ImageUrls,
            SpecificationKeys = specifications.Keys.ToList(),
            SpecificationValues = specifications.Values.ToList()
        };

        return View("CreateProduct", await BuildProductFormModelAsync(model));
    }

    [HttpPost("products/edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateProductViewModel model)
    {
        var storeId = await GetCurrentStoreIdAsync();
        if (storeId is null)
        {
            return RedirectToAction(nameof(AccountController.Register), "Account");
        }

        if (!ModelState.IsValid)
        {
            model.Id = id;
            return View("CreateProduct", await BuildProductFormModelAsync(model));
        }

        var product = await db.Products.FirstOrDefaultAsync(item => item.Id == id && item.StoreId == storeId.Value);
        if (product is null)
        {
            return NotFound();
        }

        try
        {
            var imageUrls = model.ExistingImageUrls.Where(url => !string.IsNullOrWhiteSpace(url)).ToList();
            foreach (var file in model.ImageFiles.Where(file => file.Length > 0))
            {
                imageUrls.Add(await imageService.UploadProductImageAsync(file, storeId.Value));
            }

            product.Title = model.Title;
            product.Description = model.Description;
            product.CategoryId = model.CategoryId;
            product.Price = model.Price;
            product.MinimumNegotiablePrice = model.MinimumNegotiablePrice;
            product.InventoryCount = model.InventoryCount;
            product.IsPublished = model.IsPublished;
            product.IsAvailable = model.IsAvailable;
            product.Condition = model.Condition;
            product.ImageUrls = imageUrls;
            product.SpecificationsJson = BuildSpecificationsJson(model);

            await productRepository.UpdateAsync(product);
            TempData["StatusMessage"] = "Product updated.";

            return RedirectToAction(nameof(Products));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to update product {ProductId}.", id);
            ModelState.AddModelError(string.Empty, "The product could not be updated. Please try again.");
            model.Id = id;
            return View("CreateProduct", await BuildProductFormModelAsync(model));
        }
    }

    [HttpPost("products/delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var storeId = await GetCurrentStoreIdAsync();
        if (storeId is null)
        {
            return RedirectToAction(nameof(AccountController.Register), "Account");
        }

        var product = await db.Products.FirstOrDefaultAsync(item => item.Id == id && item.StoreId == storeId.Value);
        if (product is null)
        {
            return NotFound();
        }

        product.IsPublished = false;
        product.UpdatedAt = DateTime.UtcNow;
        await productRepository.UpdateAsync(product);
        TempData["StatusMessage"] = "Product unpublished.";

        return RedirectToAction(nameof(Products));
    }

    private async Task<int?> GetCurrentStoreIdAsync()
    {
        var user = await userManager.GetUserAsync(User);
        return user?.StoreId;
    }

    private async Task<CreateProductViewModel> BuildProductFormModelAsync(CreateProductViewModel model)
    {
        var categories = await categoryRepository.GetAllAsync();
        model.Categories = categories
            .Select(category => new SelectListItem(ProductViewModelMapper.FormatCategoryName(category.Name), category.Id.ToString(), category.Id == model.CategoryId))
            .ToList();

        return model;
    }

    private static string BuildSpecificationsJson(CreateProductViewModel model)
    {
        var specifications = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var count = Math.Min(model.SpecificationKeys.Count, model.SpecificationValues.Count);

        for (var index = 0; index < count; index++)
        {
            var key = model.SpecificationKeys[index].Trim();
            var value = model.SpecificationValues[index].Trim();

            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                specifications[key] = value;
            }
        }

        return JsonSerializer.Serialize(specifications);
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

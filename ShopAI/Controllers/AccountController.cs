using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAI.Data;
using ShopAI.Helpers;
using ShopAI.Models;
using ShopAI.ViewModels;

namespace ShopAI.Controllers;

[Route("account")]
public sealed class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ApplicationDbContext db,
    ILogger<AccountController> logger) : Controller
{
    [HttpGet("register")]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var store = new Store
            {
                Name = model.StoreName,
                Slug = await GenerateUniqueStoreSlugAsync(model.StoreName),
                Description = $"The official {model.StoreName} storefront on ShopAI.",
                MerchantEmail = model.Email,
                IsActive = true
            };

            db.Stores.Add(store);
            await db.SaveChangesAsync();

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                FullName = model.FullName,
                StoreId = store.Id
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                await transaction.RollbackAsync();
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            await transaction.CommitAsync();
            await signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction(nameof(DashboardController.Index), "Dashboard");
        }
        catch (Exception exception)
        {
            await transaction.RollbackAsync();
            logger.LogError(exception, "Failed to register seller account for {Email}.", model.Email);
            ModelState.AddModelError(string.Empty, "Registration could not be completed. Please try again.");
            return View(model);
        }
    }

    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
                {
                    if (!Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }

                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction(nameof(DashboardController.Index), "Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Login failed for {Email}.", model.Email);
            ModelState.AddModelError(string.Empty, "Login could not be completed. Please try again.");
            return View(model);
        }
    }

    [HttpPost("logout")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    private async Task<string> GenerateUniqueStoreSlugAsync(string storeName)
    {
        var baseSlug = SlugHelper.GenerateSlug(storeName);
        var slug = baseSlug;
        var suffix = 2;

        while (await db.Stores.AnyAsync(store => store.Slug == slug))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }
}

using Microsoft.EntityFrameworkCore;
using ShopAI.Models;

namespace ShopAI.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var migrations = db.Database.GetMigrations();
        if (migrations.Any())
        {
            await db.Database.MigrateAsync();
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
        }

        if (!await db.Categories.AnyAsync())
        {
            db.Categories.AddRange(
                new Category
                {
                    Name = "Electronics",
                    Slug = "electronics",
                    Description = "Premium everyday electronics for connected homes and workspaces."
                },
                new Category
                {
                    Name = "Mobile Phones",
                    Slug = "mobile-phones",
                    Description = "Flagship smartphones and reliable mobile devices."
                },
                new Category
                {
                    Name = "Laptops",
                    Slug = "laptops",
                    Description = "Portable performance for creators, founders, and teams."
                },
                new Category
                {
                    Name = "Accessories",
                    Slug = "accessories",
                    Description = "Chargers, keyboards, wearables, and everyday tech essentials."
                },
                new Category
                {
                    Name = "Gaming",
                    Slug = "gaming",
                    Description = "Consoles, handhelds, and high-refresh gear for play."
                });

            await db.SaveChangesAsync();
        }

        var demoStore = await db.Stores.FirstOrDefaultAsync(store => store.Slug == "demo-electronics");
        if (demoStore is null)
        {
            demoStore = new Store
            {
                Name = "Demo Electronics",
                Slug = "demo-electronics",
                LogoUrl = "/images/logo-placeholder.svg",
                Description = "Curated devices, accessories, and gaming gear for a premium demo storefront.",
                MerchantEmail = "merchant@shopai.local",
                IsActive = true
            };

            db.Stores.Add(demoStore);
            await db.SaveChangesAsync();
        }

        if (await db.Products.AnyAsync())
        {
            logger.LogInformation("Development seed data already exists.");
            return;
        }

        var categories = await db.Categories.ToDictionaryAsync(category => category.Slug);

        db.Products.AddRange(
            new Product
            {
                StoreId = demoStore.Id,
                CategoryId = categories["mobile-phones"].Id,
                Title = "Aurelia X1 Pro Smartphone",
                Description = "A polished flagship phone with a bright OLED display, fast charging, and generous storage for everyday creators.",
                Price = 899,
                MinimumNegotiablePrice = 820,
                InventoryCount = 14,
                Condition = ProductCondition.New,
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1598327105666-5b89351aff97?auto=format&fit=crop&w=1200&q=80"
                ],
                SpecificationsJson = """{"Display":"6.7 inch OLED","RAM":"12GB","Storage":"256GB","Battery":"5000mAh"}""",
                ViewCount = 126
            },
            new Product
            {
                StoreId = demoStore.Id,
                CategoryId = categories["laptops"].Id,
                Title = "NovaBook Studio 14",
                Description = "A slim aluminum laptop with a color-accurate display, quiet keyboard, and performance tuned for design work.",
                Price = 1499,
                MinimumNegotiablePrice = 1375,
                InventoryCount = 8,
                Condition = ProductCondition.New,
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=1200&q=80"
                ],
                SpecificationsJson = """{"CPU":"Intel Core Ultra 7","RAM":"32GB","Storage":"1TB SSD","Display":"14 inch 120Hz"}""",
                ViewCount = 212
            },
            new Product
            {
                StoreId = demoStore.Id,
                CategoryId = categories["gaming"].Id,
                Title = "PulseDeck Handheld Console",
                Description = "Portable gaming with fast storage, precise controls, and a vibrant display for travel and couch play.",
                Price = 649,
                MinimumNegotiablePrice = 590,
                InventoryCount = 5,
                Condition = ProductCondition.LikeNew,
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1606144042614-b2417e99c4e3?auto=format&fit=crop&w=1200&q=80"
                ],
                SpecificationsJson = """{"Storage":"512GB","Display":"7 inch IPS","Refresh Rate":"120Hz","Connectivity":"Wi-Fi 6E"}""",
                ViewCount = 88
            },
            new Product
            {
                StoreId = demoStore.Id,
                CategoryId = categories["accessories"].Id,
                Title = "ArcCharge 3-in-1 Wireless Dock",
                Description = "A compact charging station for phone, earbuds, and watch with a tidy nightstand footprint.",
                Price = 119,
                MinimumNegotiablePrice = 95,
                InventoryCount = 24,
                Condition = ProductCondition.New,
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1603539444875-76e7684265f6?auto=format&fit=crop&w=1200&q=80"
                ],
                SpecificationsJson = """{"Output":"30W","Ports":"USB-C","Compatibility":"Qi2","Finish":"Graphite"}""",
                ViewCount = 64
            },
            new Product
            {
                StoreId = demoStore.Id,
                CategoryId = categories["electronics"].Id,
                Title = "Sonara Noise-Cancelling Headphones",
                Description = "Comfortable over-ear headphones with adaptive cancellation, deep battery life, and clean studio-inspired sound.",
                Price = 329,
                MinimumNegotiablePrice = 285,
                InventoryCount = 11,
                Condition = ProductCondition.Good,
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=1200&q=80"
                ],
                SpecificationsJson = """{"Battery":"42 hours","Bluetooth":"5.3","Charging":"USB-C","Weight":"255g"}""",
                ViewCount = 173
            },
            new Product
            {
                StoreId = demoStore.Id,
                CategoryId = categories["accessories"].Id,
                Title = "LumaKeys Mechanical Keyboard",
                Description = "Low-profile mechanical switches, hot-swap sockets, and a restrained aluminum frame for clean desk setups.",
                Price = 189,
                MinimumNegotiablePrice = 160,
                InventoryCount = 3,
                Condition = ProductCondition.LikeNew,
                ImageUrls =
                [
                    "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&w=1200&q=80"
                ],
                SpecificationsJson = """{"Layout":"75 percent","Switches":"Tactile","Connection":"Bluetooth and USB-C","Backlight":"White LED"}""",
                ViewCount = 97
            });

        await db.SaveChangesAsync();
        logger.LogInformation("Development seed data created.");
    }
}

using System.Text.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ShopAI.Models;

namespace ShopAI.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Store> Stores => Set<Store>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Conversation> Conversations => Set<Conversation>();

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var stringListComparer = new ValueComparer<List<string>>(
            (left, right) => left != null && right != null && left.SequenceEqual(right),
            value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
            value => value.ToList());

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.FullName).HasMaxLength(160).IsRequired();
            entity.Property(user => user.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasOne(user => user.Store)
                .WithMany(store => store.Users)
                .HasForeignKey(user => user.StoreId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Store>(entity =>
        {
            entity.HasIndex(store => store.Slug).IsUnique();
            entity.Property(store => store.Name).HasMaxLength(160).IsRequired();
            entity.Property(store => store.Slug).HasMaxLength(180).IsRequired();
            entity.Property(store => store.LogoUrl).HasMaxLength(500);
            entity.Property(store => store.Description).HasMaxLength(1200);
            entity.Property(store => store.MerchantEmail).HasMaxLength(256).IsRequired();
            entity.Property(store => store.FbPageAccessToken).HasMaxLength(2048);
            entity.Property(store => store.IgBusinessAccountId).HasMaxLength(256);
            entity.Property(store => store.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        });

        builder.Entity<Category>(entity =>
        {
            entity.HasIndex(category => category.Slug).IsUnique();
            entity.Property(category => category.Name).HasMaxLength(120).IsRequired();
            entity.Property(category => category.Slug).HasMaxLength(140).IsRequired();
            entity.Property(category => category.Description).HasMaxLength(800);
            entity.Property(category => category.IconSvg).HasColumnType("nvarchar(max)");
        });

        builder.Entity<Product>(entity =>
        {
            entity.HasIndex(product => product.Title);
            entity.Property(product => product.Title).HasMaxLength(220).IsRequired();
            entity.Property(product => product.Description).HasColumnType("nvarchar(max)");
            entity.Property(product => product.Price).HasPrecision(18, 2);
            entity.Property(product => product.MinimumNegotiablePrice).HasPrecision(18, 2);
            entity.Property(product => product.SpecificationsJson).HasColumnType("nvarchar(max)").HasDefaultValue("{}");
            entity.Property(product => product.Condition).HasConversion<string>().HasMaxLength(32);
            entity.Property(product => product.ViewCount).HasDefaultValue(0);
            entity.Property(product => product.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(product => product.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(product => product.ImageUrls)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    imageUrls => JsonSerializer.Serialize(imageUrls, JsonSerializerOptions.Default),
                    json => JsonSerializer.Deserialize<List<string>>(json, JsonSerializerOptions.Default) ?? new List<string>())
                .Metadata.SetValueComparer(stringListComparer);

            entity.HasOne(product => product.Store)
                .WithMany(store => store.Products)
                .HasForeignKey(product => product.StoreId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(product => product.Category)
                .WithMany(category => category.Products)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Conversation>(entity =>
        {
            entity.Property(conversation => conversation.PlatformSenderId).HasMaxLength(256).IsRequired();
            entity.Property(conversation => conversation.PlatformType).HasConversion<string>().HasMaxLength(32);
            entity.Property(conversation => conversation.CurrentAgentState).HasConversion<string>().HasMaxLength(32);
            entity.Property(conversation => conversation.MessageHistoryJson).HasColumnType("nvarchar(max)").HasDefaultValue("[]");
            entity.Property(conversation => conversation.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(conversation => conversation.UpdatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasOne(conversation => conversation.Store)
                .WithMany(store => store.Conversations)
                .HasForeignKey(conversation => conversation.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Order>(entity =>
        {
            entity.Property(order => order.CustomerName).HasMaxLength(160).IsRequired();
            entity.Property(order => order.CustomerPhone).HasMaxLength(80).IsRequired();
            entity.Property(order => order.DeliveryAddress).HasMaxLength(700).IsRequired();
            entity.Property(order => order.Notes).HasMaxLength(1200);
            entity.Property(order => order.Status).HasConversion<string>().HasMaxLength(40);
            entity.Property(order => order.TotalAmount).HasPrecision(18, 2);
            entity.Property(order => order.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasOne(order => order.Product)
                .WithMany(product => product.Orders)
                .HasForeignKey(order => order.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(order => order.Store)
                .WithMany(store => store.Orders)
                .HasForeignKey(order => order.StoreId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

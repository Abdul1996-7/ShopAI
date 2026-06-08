namespace ShopAI.Models;

public sealed class Store
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }

    public string Description { get; set; } = string.Empty;

    public string MerchantEmail { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string? FbPageAccessToken { get; set; }

    public string? IgBusinessAccountId { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();

    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

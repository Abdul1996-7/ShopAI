namespace ShopAI.Models;

public sealed class Product
{
    public int Id { get; set; }

    public int StoreId { get; set; }

    public Store Store { get; set; } = null!;

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal MinimumNegotiablePrice { get; set; }

    public int InventoryCount { get; set; }

    public bool IsPublished { get; set; } = true;

    public bool IsAvailable { get; set; } = true;

    public List<string> ImageUrls { get; set; } = [];

    public string SpecificationsJson { get; set; } = "{}";

    public ProductCondition Condition { get; set; } = ProductCondition.New;

    public int ViewCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

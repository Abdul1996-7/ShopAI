namespace ShopAI.Models;

public sealed class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? IconSvg { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopAI.Models;

namespace ShopAI.ViewModels;

public sealed record CreateProductViewModel
{
    public int? Id { get; set; }

    [Required, StringLength(220)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(5000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int CategoryId { get; set; }

    [Range(0.01, 9999999)]
    public decimal Price { get; set; }

    [Range(0, 9999999)]
    public decimal MinimumNegotiablePrice { get; set; }

    [Range(0, 100000)]
    public int InventoryCount { get; set; }

    public bool IsPublished { get; set; } = true;

    public bool IsAvailable { get; set; } = true;

    public ProductCondition Condition { get; set; } = ProductCondition.New;

    public List<IFormFile> ImageFiles { get; set; } = [];

    public List<string> ExistingImageUrls { get; set; } = [];

    public List<string> SpecificationKeys { get; set; } = [];

    public List<string> SpecificationValues { get; set; } = [];

    public IReadOnlyList<SelectListItem> Categories { get; set; } = [];
}

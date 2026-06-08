using Microsoft.AspNetCore.Identity;

namespace ShopAI.Models;

public sealed class ApplicationUser : IdentityUser
{
    public int? StoreId { get; set; }

    public Store? Store { get; set; }

    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

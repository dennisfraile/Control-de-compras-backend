namespace GroceryControl.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string GoogleId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    // Navigation
    public UserProfile Profile { get; set; } = null!;
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<InventoryEntry> InventoryEntries { get; set; } = new List<InventoryEntry>();
}

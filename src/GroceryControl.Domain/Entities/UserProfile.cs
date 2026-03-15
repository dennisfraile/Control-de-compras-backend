using GroceryControl.Domain.Enums;

namespace GroceryControl.Domain.Entities;

public class UserProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public PurchaseFrequency PurchaseFrequency { get; set; } = PurchaseFrequency.Monthly;
    public int HouseholdSize { get; set; } = 1;
    public string PreferredCurrency { get; set; } = "MXN";
    public DateTime UpdatedAtUtc { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}

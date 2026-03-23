namespace GroceryControl.Domain.Entities;

public class PurchaseBudget
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Period { get; set; } = "monthly"; // weekly, biweekly, monthly
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public User User { get; set; } = null!;
}

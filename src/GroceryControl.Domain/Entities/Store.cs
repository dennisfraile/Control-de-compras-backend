namespace GroceryControl.Domain.Entities;

public class Store
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Branch { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public bool IsGlobal { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    // Navigation
    public User? CreatedByUser { get; set; }
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}

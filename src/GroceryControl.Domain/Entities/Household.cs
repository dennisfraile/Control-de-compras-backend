namespace GroceryControl.Domain.Entities;

public class Household
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public User Owner { get; set; } = null!;
    public ICollection<HouseholdMember> Members { get; set; } = new List<HouseholdMember>();
}

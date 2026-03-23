namespace GroceryControl.Domain.Entities;

public class HouseholdMember
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = "member"; // owner, member
    public DateTime JoinedAtUtc { get; set; }
    public Household Household { get; set; } = null!;
    public User User { get; set; } = null!;
}

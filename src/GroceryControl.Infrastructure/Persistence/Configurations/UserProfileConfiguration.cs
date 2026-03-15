using GroceryControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryControl.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(up => up.Id);

        builder.Property(up => up.PurchaseFrequency)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(up => up.HouseholdSize)
            .IsRequired();

        builder.Property(up => up.PreferredCurrency)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(up => up.UserId)
            .IsUnique();
    }
}

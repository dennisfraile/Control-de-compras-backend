using GroceryControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GroceryControl.Infrastructure.Persistence.Configurations;

public class UnitTypeConfiguration : IEntityTypeConfiguration<UnitType>
{
    public void Configure(EntityTypeBuilder<UnitType> builder)
    {
        builder.ToTable("UnitTypes");

        builder.HasKey(ut => ut.Id);

        builder.Property(ut => ut.Id)
            .ValueGeneratedOnAdd();

        builder.Property(ut => ut.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ut => ut.Abbreviation)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(ut => ut.MeasurementUnit)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ut => ut.UnitCategory)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ut => ut.ConversionFactorToBase)
            .HasPrecision(18, 6);
    }
}

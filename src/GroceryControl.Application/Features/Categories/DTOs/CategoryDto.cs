using GroceryControl.Domain.Enums;

namespace GroceryControl.Application.Features.Categories.DTOs;

public record CategoryDto(
    int Id,
    string Name,
    string? Description,
    string? IconName,
    UnitCategory DefaultUnitCategory,
    int SortOrder);

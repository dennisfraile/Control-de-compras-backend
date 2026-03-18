using GroceryControl.Application.Features.Purchases.DTOs;
using MediatR;

namespace GroceryControl.Application.Features.Purchases.Queries.GetSavingsAnalysis;

public record GetSavingsAnalysisQuery : IRequest<SavingsAnalysisDto>;

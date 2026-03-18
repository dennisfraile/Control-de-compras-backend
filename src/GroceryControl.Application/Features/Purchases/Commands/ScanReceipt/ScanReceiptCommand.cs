using GroceryControl.Application.Common.Interfaces;
using MediatR;

namespace GroceryControl.Application.Features.Purchases.Commands.ScanReceipt;

public record ScanReceiptCommand(Stream ImageStream) : IRequest<ReceiptScanResult>;

public class ScanReceiptCommandHandler : IRequestHandler<ScanReceiptCommand, ReceiptScanResult>
{
    private readonly IReceiptScannerService _scanner;

    public ScanReceiptCommandHandler(IReceiptScannerService scanner)
    {
        _scanner = scanner;
    }

    public async Task<ReceiptScanResult> Handle(ScanReceiptCommand request, CancellationToken ct)
    {
        return await _scanner.ScanReceiptAsync(request.ImageStream, ct);
    }
}

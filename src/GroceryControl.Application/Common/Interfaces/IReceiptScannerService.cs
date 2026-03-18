namespace GroceryControl.Application.Common.Interfaces;

public record ScannedItem(string Name, decimal Quantity, decimal UnitPrice, decimal TotalPrice);

public record ReceiptScanResult(
    string? StoreName,
    DateTime? Date,
    List<ScannedItem> Items,
    decimal Total
);

public interface IReceiptScannerService
{
    Task<ReceiptScanResult> ScanReceiptAsync(Stream imageStream, CancellationToken ct = default);
}

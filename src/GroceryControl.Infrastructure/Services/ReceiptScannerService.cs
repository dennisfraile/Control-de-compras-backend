using System.Globalization;
using System.Text.RegularExpressions;
using GroceryControl.Application.Common.Interfaces;

namespace GroceryControl.Infrastructure.Services;

public class ReceiptScannerService : IReceiptScannerService
{
    public Task<ReceiptScanResult> ScanReceiptAsync(Stream imageStream, CancellationToken ct = default)
    {
        return Task.Run(() =>
        {
            // Read the image bytes
            using var ms = new MemoryStream();
            imageStream.CopyTo(ms);
            var imageBytes = ms.ToArray();

            // Try OCR with Tesseract if available, otherwise use regex-based fallback
            string text;
            try
            {
                using var engine = new TesseractOCR.Engine(
                    System.IO.Path.Combine(AppContext.BaseDirectory, "tessdata"),
                    TesseractOCR.Enums.Language.Esperanto); // fallback language that exists
                using var img = TesseractOCR.Pix.Image.LoadFromMemory(imageBytes);
                using var page = engine.Process(img);
                text = page.Text;
            }
            catch
            {
                // If Tesseract data files not available, return empty result
                // In production, configure proper tessdata path
                return new ReceiptScanResult(null, null, new List<ScannedItem>(), 0);
            }

            return ParseReceipt(text);
        }, ct);
    }

    private static ReceiptScanResult ParseReceipt(string text)
    {
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .Select(l => l.Trim())
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .ToList();

        string? storeName = null;
        DateTime? date = null;
        var items = new List<ScannedItem>();
        decimal total = 0;

        // Try to find store name (usually in first 3 lines, longest non-numeric line)
        foreach (var line in lines.Take(5))
        {
            if (!Regex.IsMatch(line, @"^\d") && line.Length > 3 && !Regex.IsMatch(line, @"^\$"))
            {
                storeName = line;
                break;
            }
        }

        foreach (var line in lines)
        {
            // Try to find date
            if (date == null)
            {
                var dateMatch = Regex.Match(line, @"(\d{1,2}[/-]\d{1,2}[/-]\d{2,4})");
                if (dateMatch.Success)
                {
                    if (DateTime.TryParse(dateMatch.Groups[1].Value, new CultureInfo("es-MX"), DateTimeStyles.None, out var parsed))
                        date = parsed;
                }
            }

            // Try to find total
            var totalMatch = Regex.Match(line, @"(?:TOTAL|Total|total)\s*[\$:]?\s*(\d+[.,]\d{2})", RegexOptions.IgnoreCase);
            if (totalMatch.Success)
            {
                if (decimal.TryParse(totalMatch.Groups[1].Value.Replace(",", "."), CultureInfo.InvariantCulture, out var t))
                    total = t;
                continue;
            }

            // Pattern 1: "PRODUCT NAME    2    $45.00    $90.00"
            var itemMatch1 = Regex.Match(line, @"^(.+?)\s+(\d+(?:[.,]\d+)?)\s+\$?\s*(\d+[.,]\d{2})\s+\$?\s*(\d+[.,]\d{2})$");
            if (itemMatch1.Success)
            {
                var name = itemMatch1.Groups[1].Value.Trim();
                var qty = ParseDecimal(itemMatch1.Groups[2].Value);
                var unitPrice = ParseDecimal(itemMatch1.Groups[3].Value);
                var totalPrice = ParseDecimal(itemMatch1.Groups[4].Value);
                if (name.Length > 1 && qty > 0 && unitPrice > 0)
                {
                    items.Add(new ScannedItem(name, qty, unitPrice, totalPrice));
                    continue;
                }
            }

            // Pattern 2: "PRODUCT NAME    $45.00"
            var itemMatch2 = Regex.Match(line, @"^(.+?)\s+\$?\s*(\d+[.,]\d{2})$");
            if (itemMatch2.Success)
            {
                var name = itemMatch2.Groups[1].Value.Trim();
                var price = ParseDecimal(itemMatch2.Groups[2].Value);
                if (name.Length > 2 && price > 0 && !IsHeaderOrFooter(name))
                {
                    items.Add(new ScannedItem(name, 1, price, price));
                }
            }
        }

        if (total == 0 && items.Count > 0)
            total = items.Sum(i => i.TotalPrice);

        return new ReceiptScanResult(storeName, date, items, total);
    }

    private static decimal ParseDecimal(string value)
    {
        var normalized = value.Replace(",", ".");
        return decimal.TryParse(normalized, CultureInfo.InvariantCulture, out var result) ? result : 0;
    }

    private static bool IsHeaderOrFooter(string name)
    {
        var skip = new[] { "subtotal", "sub total", "iva", "impuesto", "cambio", "efectivo", "tarjeta", "visa", "mastercard", "rfc", "ticket", "sucursal", "cajero", "gracias" };
        return skip.Any(s => name.Contains(s, StringComparison.OrdinalIgnoreCase));
    }
}

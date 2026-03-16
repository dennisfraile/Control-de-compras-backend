using ClosedXML.Excel;
using GroceryControl.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Purchases.Queries.ExportPurchases;

public class ExportPurchasesQueryHandler : IRequestHandler<ExportPurchasesQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ExportPurchasesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<byte[]> Handle(ExportPurchasesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Purchases
            .AsNoTracking()
            .Include(p => p.Store)
            .Include(p => p.Items)
                .ThenInclude(i => i.Product)
            .Where(p => p.UserId == _currentUser.UserId)
            .AsQueryable();

        if (request.FromDate.HasValue)
            query = query.Where(p => p.PurchaseDateUtc >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(p => p.PurchaseDateUtc <= request.ToDate.Value);

        var purchases = await query
            .OrderByDescending(p => p.PurchaseDateUtc)
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();

        // Sheet 1: Resumen de Compras
        var summarySheet = workbook.Worksheets.Add("Resumen de Compras");
        summarySheet.Cell(1, 1).Value = "Fecha";
        summarySheet.Cell(1, 2).Value = "Tienda";
        summarySheet.Cell(1, 3).Value = "Total";
        summarySheet.Cell(1, 4).Value = "Cantidad de Items";

        var headerRow = summarySheet.Row(1);
        headerRow.Style.Font.Bold = true;

        for (int i = 0; i < purchases.Count; i++)
        {
            var p = purchases[i];
            int row = i + 2;
            summarySheet.Cell(row, 1).Value = p.PurchaseDateUtc.ToString("dd/MM/yyyy");
            summarySheet.Cell(row, 2).Value = p.Store.Name;
            summarySheet.Cell(row, 3).Value = p.TotalAmount;
            summarySheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
            summarySheet.Cell(row, 4).Value = p.Items.Count;
        }

        summarySheet.Columns().AdjustToContents();

        // Sheet 2: Detalle de Items
        var detailSheet = workbook.Worksheets.Add("Detalle de Items");
        detailSheet.Cell(1, 1).Value = "Fecha";
        detailSheet.Cell(1, 2).Value = "Tienda";
        detailSheet.Cell(1, 3).Value = "Producto";
        detailSheet.Cell(1, 4).Value = "Cantidad";
        detailSheet.Cell(1, 5).Value = "Precio Unitario";
        detailSheet.Cell(1, 6).Value = "Total";

        var detailHeaderRow = detailSheet.Row(1);
        detailHeaderRow.Style.Font.Bold = true;

        int detailRow = 2;
        foreach (var p in purchases)
        {
            foreach (var item in p.Items)
            {
                detailSheet.Cell(detailRow, 1).Value = p.PurchaseDateUtc.ToString("dd/MM/yyyy");
                detailSheet.Cell(detailRow, 2).Value = p.Store.Name;
                detailSheet.Cell(detailRow, 3).Value = item.Product.Name;
                detailSheet.Cell(detailRow, 4).Value = item.Quantity;
                detailSheet.Cell(detailRow, 4).Style.NumberFormat.Format = "#,##0.##";
                detailSheet.Cell(detailRow, 5).Value = item.UnitPrice;
                detailSheet.Cell(detailRow, 5).Style.NumberFormat.Format = "#,##0.00";
                detailSheet.Cell(detailRow, 6).Value = item.TotalPrice;
                detailSheet.Cell(detailRow, 6).Style.NumberFormat.Format = "#,##0.00";
                detailRow++;
            }
        }

        detailSheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

using ClosedXML.Excel;
using GroceryControl.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GroceryControl.Application.Features.Inventory.Queries.ExportInventory;

public class ExportInventoryQueryHandler : IRequestHandler<ExportInventoryQuery, byte[]>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ExportInventoryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<byte[]> Handle(ExportInventoryQuery request, CancellationToken cancellationToken)
    {
        var entries = await _context.InventoryEntries
            .AsNoTracking()
            .Include(ie => ie.Product)
                .ThenInclude(p => p.Category)
            .Include(ie => ie.UnitType)
            .Where(ie => ie.UserId == _currentUser.UserId)
            .OrderBy(ie => ie.Product.Name)
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();

        var sheet = workbook.Worksheets.Add("Inventario");
        sheet.Cell(1, 1).Value = "Producto";
        sheet.Cell(1, 2).Value = "Categoria";
        sheet.Cell(1, 3).Value = "Cantidad Actual";
        sheet.Cell(1, 4).Value = "Unidad";
        sheet.Cell(1, 5).Value = "Umbral Minimo";
        sheet.Cell(1, 6).Value = "Estado";

        var headerRow = sheet.Row(1);
        headerRow.Style.Font.Bold = true;

        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            int row = i + 2;
            sheet.Cell(row, 1).Value = e.Product.Name;
            sheet.Cell(row, 2).Value = e.Product.Category.Name;
            sheet.Cell(row, 3).Value = e.CurrentQuantity;
            sheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.##";
            sheet.Cell(row, 4).Value = e.UnitType.Abbreviation;
            sheet.Cell(row, 5).Value = e.MinimumThreshold;
            sheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0.##";

            var status = e.CurrentQuantity <= e.MinimumThreshold ? "Bajo" : "OK";
            sheet.Cell(row, 6).Value = status;

            if (status == "Bajo")
                sheet.Cell(row, 6).Style.Font.FontColor = XLColor.Red;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

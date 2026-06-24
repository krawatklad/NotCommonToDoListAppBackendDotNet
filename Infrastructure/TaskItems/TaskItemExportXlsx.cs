using Application.TaskItems.Interfaces;
using ClosedXML.Excel;
using Domain.Entities;

namespace Infrastructure.TaskItems;

public class TaskItemExportXlsx : ITaskItemExportXlsx
{
    public byte[] Export(IEnumerable<TaskItem> taskItems)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Task Items");

        AddHeaders(worksheet);
        FillWithData(worksheet, taskItems);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        
        return stream.ToArray();
    }

    public string GetMimeType()
    {
        return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }

    private static void AddHeaders(IXLWorksheet worksheet)
    {
        // TODO in the future maybe add headers translation
        worksheet.Cell(1, 1).Value = "Title";
        worksheet.Cell(1, 2).Value = "Description";
        worksheet.Cell(1, 3).Value = "Status";
        worksheet.Cell(1, 4).Value = "Deadline (UTC)";
        worksheet.Cell(1, 5).Value = "Created At (UTC)";

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
    }

    private static void FillWithData(IXLWorksheet worksheet, IEnumerable<TaskItem> taskItems)
    {
        var row = 2;
        foreach (var item in taskItems)
        {
            worksheet.Cell(row, 1).Value = item.Title;
            worksheet.Cell(row, 2).Value = item.Description;
            worksheet.Cell(row, 3).Value = item.Status.ToString();
            worksheet.Cell(row, 4).Value = item.Deadline.UtcDateTime;
            worksheet.Cell(row, 5).Value = item.CreatedAt.UtcDateTime;
            ++row;
        }

        worksheet.Columns().AdjustToContents();
    }
}
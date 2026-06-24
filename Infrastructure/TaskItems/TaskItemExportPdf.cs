using Application.TaskItems.Interfaces;
using Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.TaskItems;

public class TaskItemExportPdf : ITaskItemExportPdf
{
    public byte[] Export(IEnumerable<TaskItem> taskItems)
    {
        // TODO: Depends on project scale/budget (free for individuals)
        //  Alternative solution could be razor + puppeteer/playwright but it runs on Chromium,
        //  so browser has to be installed on the server and it is more resource intensive.
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Text("Task Items Export")
                    .SemiBold()
                    .FontSize(20);

                page.Content()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Title");
                            header.Cell().Element(CellStyle).Text("Description");
                            header.Cell().Element(CellStyle).Text("Status");
                            header.Cell().Element(CellStyle).Text("Deadline (UTC)");
                            header.Cell().Element(CellStyle).Text("Created At (UTC)");
                            return;

                            static IContainer CellStyle(IContainer container)
                            {
                                return container
                                    .Background(Colors.Grey.Lighten2)
                                    .Border(1)
                                    .Padding(5);
                            }
                        });

                        foreach (var item in taskItems)
                        {
                            table.Cell().Element(DataCellStyle).Text(item.Title);
                            table.Cell().Element(DataCellStyle).Text(item.Description);
                            table.Cell().Element(DataCellStyle).Text(item.Status.ToString());
                            table.Cell().Element(DataCellStyle).Text(item.Deadline.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
                            table.Cell().Element(DataCellStyle).Text(item.CreatedAt.UtcDateTime.ToString("yyyy-MM-dd HH:mm"));
                        }

                        return;

                        static IContainer DataCellStyle(IContainer container)
                        {
                            return container
                                .Border(1)
                                .Padding(5);
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
            });
        }).GeneratePdf();
    }

    public string GetMimeType()
    {
        return "application/pdf";
    }
}
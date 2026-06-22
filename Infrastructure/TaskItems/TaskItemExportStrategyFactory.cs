using Application.Common.Enums;
using Application.TaskItems.Interfaces;

namespace Infrastructure.TaskItems;

public class TaskItemExportStrategyFactory(ITaskItemExportXlsx taskItemExportXlsx, 
    ITaskItemExportPdf taskItemExportPdf) : ITaskItemExportStrategyFactory
{
    public ITaskItemExportStrategy GetStrategy(ExportFormat format)
    {
        return format switch{
            ExportFormat.Pdf => taskItemExportPdf,
            ExportFormat.Xlsx => taskItemExportXlsx,
            _ => throw new NotImplementedException($"Export format {format} is not supported.")
        };
    }
}

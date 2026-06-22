using Application.Common.Enums;

namespace Application.TaskItems.Interfaces;

public interface ITaskItemExportStrategyFactory
{
    ITaskItemExportStrategy GetStrategy(ExportFormat format);
}

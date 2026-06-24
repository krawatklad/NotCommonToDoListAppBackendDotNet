using Application.Abstractions;
using Application.Common.Persistence;
using Application.TaskItems.DTOs;
using Application.TaskItems.Interfaces;

namespace Application.TaskItems.Queries.ExportTaskItems;

public class ExportTaskItemsQueryHandler(
    ITaskItemRepository taskItemRepository,
    ITaskItemExportStrategyFactory taskItemExportStrategyFactory)
    : IQueryHandler<ExportTaskItemsQuery, ExportedTaskItemsDto>
{
    public async Task<ExportedTaskItemsDto> Handle(ExportTaskItemsQuery query, CancellationToken cancellationToken = default)
    {
        var taskItems = await taskItemRepository.GetAllAsync(
            query.UserId,
            query.Search,
            query.Statuses,
            query.OrderBy,
            query.SortDirection,
            cancellationToken);
        
        var strategy = taskItemExportStrategyFactory.GetStrategy(query.Format);
        
        return new ExportedTaskItemsDto(Content: strategy.Export(taskItems), MimeType: strategy.GetMimeType());
    }
}
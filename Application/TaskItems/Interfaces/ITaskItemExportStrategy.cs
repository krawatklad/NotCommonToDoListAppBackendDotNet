using Domain.Entities;

namespace Application.TaskItems.Interfaces;

public interface ITaskItemExportStrategy
{
    byte[] Export(IEnumerable<TaskItem> taskItems);
    
    string GetMimeType();
}

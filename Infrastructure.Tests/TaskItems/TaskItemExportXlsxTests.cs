using Domain.Entities;
using Domain.Enums;
using Infrastructure.TaskItems;

namespace Infrastructure.Tests.TaskItems;

public class TaskItemExportXlsxTests
{
    private readonly TaskItemExportXlsx _exporter = new();

    [Fact]
    public void Export_ShouldReturnByteArray()
    {
        // Arrange
        var taskItems = new List<TaskItem>
        {
            new(
                "Task 1",
                "Desc 1",
                TaskItemStatus.New,
                DateTimeOffset.UtcNow,
                Guid.NewGuid(),
                DateTimeOffset.UtcNow
            )
        };

        // Act
        var result = _exporter.Export(taskItems);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetMimeType_ShouldReturnXlsxMimeType()
    {
        // Act
        var result = _exporter.GetMimeType();

        // Assert
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result);
    }
}
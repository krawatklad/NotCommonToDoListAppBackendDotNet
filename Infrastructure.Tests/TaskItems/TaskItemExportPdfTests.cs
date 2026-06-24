using Domain.Entities;
using Domain.Enums;
using Infrastructure.TaskItems;
using QuestPDF;
using QuestPDF.Infrastructure;

namespace Infrastructure.Tests.TaskItems;

public class TaskItemExportPdfTests
{
    private readonly TaskItemExportPdf _exporter;

    public TaskItemExportPdfTests()
    {
        Settings.License = LicenseType.Community;
        _exporter = new TaskItemExportPdf();
    }

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
    public void GetMimeType_ShouldReturnPdfMimeType()
    {
        // Act
        var result = _exporter.GetMimeType();

        // Assert
        Assert.Equal("application/pdf", result);
    }
}
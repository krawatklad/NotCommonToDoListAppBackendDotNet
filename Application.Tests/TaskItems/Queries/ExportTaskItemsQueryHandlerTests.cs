using Application.Common.Enums;
using Application.Common.Persistence;
using Application.TaskItems.Interfaces;
using Application.TaskItems.Queries.ExportTaskItems;
using Application.TaskItems.Queries.GetTaskItems;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Tests.TaskItems.Queries;

public class ExportTaskItemsQueryHandlerTests
{
    private static readonly DateTimeOffset FixedUtcNow = new(2026, 6, 18, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid UserId = new("00000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task Handle_WhenCalled_ReturnsExportedTaskItems()
    {
        // Arrange
        var taskItems = new List<TaskItem>
        {
            new("Task 1", null, TaskItemStatus.New, FixedUtcNow.AddDays(1), UserId, FixedUtcNow),
            new("Task 2", null, TaskItemStatus.New, FixedUtcNow.AddDays(1), UserId, FixedUtcNow)
        };
        
        var repositoryMock = new Mock<ITaskItemRepository>();
        repositoryMock.Setup(r => r.GetAllAsync(
                UserId, 
                It.IsAny<string>(), 
                It.IsAny<TaskItemStatus[]>(), 
                It.IsAny<TaskItemOrderBy>(), 
                It.IsAny<SortDirection>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskItems);
        
        var strategyMock = new Mock<ITaskItemExportStrategy>();
        var content = new byte[] { 1, 2, 3 };
        const string mimeType = "text/csv";
        strategyMock.Setup(s => s.Export(taskItems)).Returns(content);
        strategyMock.Setup(s => s.GetMimeType()).Returns(mimeType);
        
        var factoryMock = new Mock<ITaskItemExportStrategyFactory>();
        factoryMock.Setup(f => f.GetStrategy(ExportFormat.Pdf)).Returns(strategyMock.Object);

        var handler = new ExportTaskItemsQueryHandler(repositoryMock.Object, factoryMock.Object);
        var query = new ExportTaskItemsQuery(UserId: UserId, Format: ExportFormat.Pdf);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(content, result.Content);
        Assert.Equal(mimeType, result.MimeType);
        strategyMock.Verify(s => s.Export(taskItems), Times.Once);
    }
}

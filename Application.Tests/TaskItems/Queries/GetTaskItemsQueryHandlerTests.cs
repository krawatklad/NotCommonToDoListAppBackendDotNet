using Application.Common;
using Application.Common.Enums;
using Application.Common.Persistence;
using Application.TaskItems.Queries.GetTaskItems;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Tests.TaskItems.Queries;

public class GetTaskItemsQueryHandlerTests
{
    private static readonly DateTimeOffset FixedUtcNow = new(2026, 6, 18, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid UserId = new("00000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task Handle_WhenCalled_ReturnsPagedTaskItems()
    {
        // Arrange
        var taskItems = new List<TaskItem>
        {
            new("Task 1", null, TaskItemStatus.New, FixedUtcNow.AddDays(1), UserId, FixedUtcNow),
            new("Task 2", null, TaskItemStatus.New, FixedUtcNow.AddDays(1), UserId, FixedUtcNow)
        };
        
        var paginatedList = new PaginatedList<TaskItem>(taskItems, 1, taskItems.Count, 10);
        
        var repositoryMock = new Mock<ITaskItemRepository>();
        repositoryMock.Setup(r => r.GetPagedAsync(
                UserId, 
                1, 
                10, 
                It.IsAny<string>(), 
                It.IsAny<TaskItemStatus[]>(), 
                It.IsAny<TaskItemOrderBy>(), 
                It.IsAny<SortDirection>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedList);
            
        var handler = new GetTaskItemsQueryHandler(repositoryMock.Object);
        var query = new GetTaskItemsQuery(UserId: UserId, PageNumber: 1, PageSize: 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(taskItems.Count, result.Items.Count);
        Assert.Equal(taskItems.Count, result.TotalCount);
        Assert.Equal(taskItems, result.Items);
    }
}

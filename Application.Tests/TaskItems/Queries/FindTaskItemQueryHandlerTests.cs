using Application.Common.Exceptions;
using Application.Common.Persistence;
using Application.TaskItems.Queries.FindTaskItem;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Tests.TaskItems.Queries;

public class FindTaskItemQueryHandlerTests
{
    private static readonly DateTimeOffset FixedUtcNow = new(2026, 6, 18, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid UserId = new("00000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherUserId = new("00000000-0000-0000-0000-000000000002");
    private static readonly Guid TaskItemId = new("00000000-0000-0000-0000-000000000003");

    [Fact]
    public async Task Handle_WhenTaskItemNotFound_ReturnsNull()
    {
        // Arrange
        var repositoryMock = new Mock<ITaskItemRepository>();
        repositoryMock.Setup(r => r.FindByIdAsync(TaskItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);
            
        var handler = new FindTaskItemQueryHandler(repositoryMock.Object);
        var query = new FindTaskItemQuery(TaskItemId, UserId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenUserNotAuthorized_ThrowsForbiddenException()
    {
        // Arrange
        var taskItem = new TaskItem(
            title: "Task",
            description: null,
            status: TaskItemStatus.New,
            deadline: FixedUtcNow.AddDays(1),
            createdById: OtherUserId,
            createdAt: FixedUtcNow)
        {
            Id = TaskItemId
        };

        var repositoryMock = new Mock<ITaskItemRepository>();
        repositoryMock.Setup(r => 
                r.FindByIdAsync(TaskItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskItem);
            
        var handler = new FindTaskItemQueryHandler(repositoryMock.Object);
        var query = new FindTaskItemQuery(TaskItemId, UserId);

        // Act
        var action = () => handler.Handle(query, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<ForbiddenException>(action);
    }

    [Fact]
    public async Task Handle_WhenAuthorized_ReturnsTaskItem()
    {
        // Arrange
        var taskItem = new TaskItem(
            title: "Task",
            description: null,
            status: TaskItemStatus.New,
            deadline: FixedUtcNow,
            createdById: UserId,
            createdAt: FixedUtcNow)
        {
            Id = TaskItemId
        };

        var repositoryMock = new Mock<ITaskItemRepository>();
        repositoryMock.Setup(r => 
                r.FindByIdAsync(TaskItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskItem);
            
        var handler = new FindTaskItemQueryHandler(repositoryMock.Object);
        var query = new FindTaskItemQuery(TaskItemId, UserId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(taskItem, result);
    }
}
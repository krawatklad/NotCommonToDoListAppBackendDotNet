using Application.Common.Exceptions;
using Application.Common.Persistence;
using Application.TaskItems.Commands.UpdateTaskItem;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Tests.TaskItems.Commands;

public class UpdateTaskItemCommandHandlerTests
{
    private readonly UpdateTaskItemCommandHandler _handler;
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly Mock<TimeProvider> _timeProviderMock;

    public UpdateTaskItemCommandHandlerTests()
    {
        _taskItemRepositoryMock = new Mock<ITaskItemRepository>();
        _timeProviderMock = new Mock<TimeProvider>();
        
        _handler = new UpdateTaskItemCommandHandler(
            _taskItemRepositoryMock.Object,
            _timeProviderMock.Object);
    }

    [Fact]
    public async Task Handle_WhenTaskItemNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateTaskItemCommand(Guid.NewGuid(), "New Title", "New Desc", TaskItemStatus.InProgress, DateTimeOffset.UtcNow, Guid.NewGuid());
        _taskItemRepositoryMock.Setup(x => x.FindByIdAsync(command.TaskItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldUpdateTaskItem()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var originalCreatedAt = new DateTimeOffset(2026, 6, 17, 12, 0, 0, TimeSpan.Zero);
        var existingTask = new TaskItem("Old Title", "Old Desc", TaskItemStatus.New, DateTimeOffset.UtcNow, Guid.NewGuid(), originalCreatedAt);
        
        var command = new UpdateTaskItemCommand(taskId, "New Title", "New Desc", TaskItemStatus.InProgress, DateTimeOffset.UtcNow.AddDays(1), Guid.NewGuid());
        var now = new DateTimeOffset(2026, 6, 18, 12, 0, 0, TimeSpan.Zero);

        _taskItemRepositoryMock.Setup(x => x.FindByIdAsync(command.TaskItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTask);
        _timeProviderMock.Setup(x => x.GetUtcNow())
            .Returns(now);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.Title, result.Title);
        Assert.Equal(command.Description, result.Description);
        Assert.Equal(command.Status, result.Status);
        Assert.Equal(command.Deadline.ToUniversalTime(), result.Deadline);
        Assert.Equal(now, result.UpdatedAt);
        Assert.Equal(command.UserId, result.UpdatedById);
        Assert.Equal(originalCreatedAt, result.CreatedAt);
    }
}

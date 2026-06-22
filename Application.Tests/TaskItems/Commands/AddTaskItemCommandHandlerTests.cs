using Application.Common.Persistence;
using Application.TaskItems.Commands.AddTaskItem;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Tests.TaskItems.Commands;

public class AddTaskItemCommandHandlerTests
{
    private static readonly DateTimeOffset FixedUtcNow = new(2026, 6, 18, 12, 0, 0, TimeSpan.Zero);
    private static readonly Guid UserId = new("00000000-0000-0000-0000-000000000001");

    [Fact]
    public async Task Handle_WhenValidCommand_ShouldAddTaskItemAndReturnId()
    {
        // Arrange
        var repositoryMock = new Mock<ITaskItemRepository>();
        var timeProviderMock = new Mock<TimeProvider>();
        timeProviderMock.Setup(x => x.GetUtcNow()).Returns(FixedUtcNow);
        
        var handler = new AddTaskItemCommandHandler(repositoryMock.Object, timeProviderMock.Object);
        var command = new AddTaskItemCommand(
            Title: "Test Task",
            Description: "Test Description",
            Status: TaskItemStatus.New,
            Deadline: FixedUtcNow.AddDays(1),
            UserId: UserId
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        repositoryMock.Verify(r => r.AddAsync(It.Is<TaskItem>(t => 
            t.Title == command.Title &&
            t.Description == command.Description &&
            t.Status == command.Status &&
            t.Deadline == command.Deadline &&
            t.CreatedById == UserId &&
            t.CreatedAt == FixedUtcNow
        ), It.IsAny<CancellationToken>()), Times.Once);
    }
}

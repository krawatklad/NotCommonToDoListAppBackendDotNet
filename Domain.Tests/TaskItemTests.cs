using Domain.Entities;
using Domain.Enums;

namespace Domain.Tests;

public class TaskItemTests
{
    [Fact]
    public void Title_Should_Handle_Empty_String()
    {
        // Arrange
        const string title = "";
        
        // Act
        var taskItem = new TaskItem
        (
            title: title,
            description: null,
            status: TaskItemStatus.New,
            deadline: DateTimeOffset.UtcNow,
            createdById: Guid.NewGuid(),
            createdAt: DateTimeOffset.UtcNow
        );

        // Assert
        Assert.Equal("", taskItem.Title);
    }
    
    [Fact]
    public void Title_Should_Capitalize_First_Letter()
    {
        // Arrange
        const string title = "test task";
        
        // Act
        var taskItem = new TaskItem
        (
            title: title,
            description: null,
            status: TaskItemStatus.New,
            deadline: DateTimeOffset.UtcNow,
            createdById: Guid.NewGuid(),
            createdAt: DateTimeOffset.UtcNow
        );

        // Assert
        Assert.Equal("Test task", taskItem.Title);
    }
    
    [Fact]
    public void Title_Should_Remain_Unchanged_When_First_Letter_Is_Already_Uppercase()
    {
        // Arrange
        const string title = "Test task";
        
        // Act
        var taskItem = new TaskItem
        (
            title: title,
            description: null,
            status: TaskItemStatus.New,
            deadline: DateTimeOffset.UtcNow,
            createdById: Guid.NewGuid(),
            createdAt: DateTimeOffset.UtcNow
        );

        // Assert
        Assert.Equal(title, taskItem.Title);
    }
}
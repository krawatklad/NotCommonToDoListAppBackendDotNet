using System.Security.Claims;
using Api.Contracts.Request;
using Api.Contracts.Response;
using Api.Controllers;
using Application.Abstractions;
using Application.Common;
using Application.TaskItems.Commands.AddTaskItem;
using Application.TaskItems.Commands.DeleteTaskItem;
using Application.TaskItems.Commands.UpdateTaskItem;
using Application.TaskItems.DTOs;
using Application.TaskItems.Queries.ExportTaskItems;
using Application.TaskItems.Queries.FindTaskItem;
using Application.TaskItems.Queries.GetTaskItems;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.Tests.Controllers;

public class TaskItemControllerTests
{
    private readonly Mock<ICommandHandler<AddTaskItemCommand, Guid>> _addTaskItemHandlerMock;
    private readonly TaskItemController _controller;
    private readonly Mock<ICommandHandler<DeleteTaskItemCommand, Unit>> _deleteTaskItemHandlerMock;
    private readonly Mock<IQueryHandler<ExportTaskItemsQuery, ExportedTaskItemsDto>> _exportTaskItemsHandlerMock;
    private readonly Mock<IQueryHandler<FindTaskItemQuery, TaskItem?>> _findTaskItemHandlerMock;
    private readonly Mock<IQueryHandler<GetTaskItemsQuery, PaginatedList<TaskItem>>> _getTaskItemsHandlerMock;
    private readonly Mock<ICommandHandler<UpdateTaskItemCommand, TaskItem>> _updateTaskItemHandlerMock;
    private readonly Guid _userId = new("00000000-0000-0000-0000-000000000001");

    public TaskItemControllerTests()
    {
        _addTaskItemHandlerMock = new Mock<ICommandHandler<AddTaskItemCommand, Guid>>();
        _updateTaskItemHandlerMock = new Mock<ICommandHandler<UpdateTaskItemCommand, TaskItem>>();
        _deleteTaskItemHandlerMock = new Mock<ICommandHandler<DeleteTaskItemCommand, Unit>>();
        _findTaskItemHandlerMock = new Mock<IQueryHandler<FindTaskItemQuery, TaskItem?>>();
        _getTaskItemsHandlerMock = new Mock<IQueryHandler<GetTaskItemsQuery, PaginatedList<TaskItem>>>();
        _exportTaskItemsHandlerMock = new Mock<IQueryHandler<ExportTaskItemsQuery, ExportedTaskItemsDto>>();

        _controller = new TaskItemController(
            _addTaskItemHandlerMock.Object,
            _updateTaskItemHandlerMock.Object,
            _deleteTaskItemHandlerMock.Object,
            _findTaskItemHandlerMock.Object,
            _getTaskItemsHandlerMock.Object,
            _exportTaskItemsHandlerMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
        ], "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task GetTaskItems_ShouldReturnOkWithPaginatedList()
    {
        // Arrange
        var request = new GetTaskItemsRequest();
        var taskItems = new List<TaskItem>
        {
            new(
                "Title", 
                "Desc", 
                TaskItemStatus.New,
                DateTimeOffset.UtcNow,
                _userId, 
                DateTimeOffset.UtcNow
            )
        };
        var paginatedList = new PaginatedList<TaskItem>(taskItems, 1, 1, 10);
        
        _getTaskItemsHandlerMock.Setup(x => 
                x.Handle(It.IsAny<GetTaskItemsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _controller.GetTaskItems(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<PagedTaskItemsResponse>(okResult.Value);
        Assert.Single(response.Items);
        Assert.Equal(1, response.Count);
    }

    [Fact]
    public async Task AddTaskItem_ShouldReturnOkWithId()
    {
        // Arrange
        var request = new AddTaskItemRequest("Title", "Desc", TaskItemStatus.New, DateTimeOffset.UtcNow);
        var taskId = new Guid("00000000-0000-0000-0000-000000000002");
        _addTaskItemHandlerMock.Setup(x => 
                x.Handle(It.IsAny<AddTaskItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskId);

        // Act
        var result = await _controller.AddTaskItem(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AddTaskItemResponse>(okResult.Value);
        Assert.Equal(taskId, response.Id);
    }

    [Fact]
    public async Task FindTaskItem_WhenExists_ShouldReturnOk()
    {
        // Arrange
        var taskId = new Guid("00000000-0000-0000-0000-000000000002");
        var task = new TaskItem(
            "Title", 
            "Desc",
            TaskItemStatus.New, 
            DateTimeOffset.UtcNow,
            _userId, 
            DateTimeOffset.UtcNow
        );
        _findTaskItemHandlerMock.Setup(x => 
                x.Handle(It.IsAny<FindTaskItemQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        // Act
        var result = await _controller.FindTaskItem(taskId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<GetTaskItemResponse>(okResult.Value);
        Assert.Equal(task.Title, response.Title);
    }

    [Fact]
    public async Task FindTaskItem_WhenNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var taskId = new Guid("00000000-0000-0000-0000-000000000002");
        _findTaskItemHandlerMock.Setup(x => 
                x.Handle(It.IsAny<FindTaskItemQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _controller.FindTaskItem(taskId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateTaskItem_ShouldReturnOk()
    {
        // Arrange
        var taskId = new Guid("00000000-0000-0000-0000-000000000002");
        var request = new UpdateTaskItemRequest(
            "New Title", 
            "New Desc",
            TaskItemStatus.InProgress,
            DateTimeOffset.UtcNow
        );
        var updatedTask = new TaskItem(
            "New Title", 
            "New Desc",
            TaskItemStatus.InProgress, 
            DateTimeOffset.UtcNow,
            _userId, 
            DateTimeOffset.UtcNow
        );
        
        _updateTaskItemHandlerMock.Setup(x => 
                x.Handle(It.IsAny<UpdateTaskItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTask);

        // Act
        var result = await _controller.UpdateTaskItem(taskId, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<TaskItemResponse>(okResult.Value);
        Assert.Equal(request.Title, response.Title);
    }

    [Fact]
    public async Task DeleteTaskItem_ShouldReturnNoContent()
    {
        // Arrange
        var taskId = new Guid("00000000-0000-0000-0000-000000000002");
        _deleteTaskItemHandlerMock.Setup(x => 
                x.Handle(It.IsAny<DeleteTaskItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        // Act
        var result = await _controller.DeleteTaskItem(taskId);

        // Assert
        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task ExportTaskItems_ShouldReturnFileResult()
    {
        // Arrange
        var request = new ExportTaskItemsRequest();
        var exportedDto = new ExportedTaskItemsDto([1, 2, 3], "application/pdf");
        _exportTaskItemsHandlerMock.Setup(x => 
                x.Handle(It.IsAny<ExportTaskItemsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exportedDto);

        // Act
        var result = await _controller.ExportTaskItems(request);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/pdf", fileResult.ContentType);
        Assert.Equal(exportedDto.Content, fileResult.FileContents);
    }
}
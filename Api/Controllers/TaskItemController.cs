using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.Contracts.Request;
using Api.Contracts.Response;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("tasks")]
[Authorize]
public class TaskItemController(
    ICommandHandler<AddTaskItemCommand, Guid> addTaskItemHandler,
    ICommandHandler<UpdateTaskItemCommand, TaskItem> updateTaskItemHandler,
    ICommandHandler<DeleteTaskItemCommand, Unit> deleteTaskItemHandler,
    IQueryHandler<FindTaskItemQuery, TaskItem?> findTaskItemHandler,
    IQueryHandler<GetTaskItemsQuery, PaginatedList<TaskItem>> getTaskItemsHandler,
    IQueryHandler<ExportTaskItemsQuery, ExportedTaskItemsDto> exportTaskItemsHandler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedTaskItemsResponse>> GetTaskItems(
        [FromQuery] GetTaskItemsRequest request)
    {
        if (!TryGetUserId(User, out var userId))
        {
            return Unauthorized("Missing or invalid user id claim.");
        }

        var query = new GetTaskItemsQuery(
            UserId: userId,
            PageNumber: request.Page,
            PageSize: request.Limit,
            Search: request.Search,
            Statuses: request.Statuses,
            OrderBy: request.OrderBy,
            SortDirection: request.OrderByDirection);

        var result = await getTaskItemsHandler.Handle(query, HttpContext.RequestAborted);

        var response = new PagedTaskItemsResponse(
            Items: result.Items.Select(t => new TaskItemResponse(
                Id: t.Id,
                Title: t.Title,
                Description: t.Description,
                Status: t.Status,
                Deadline: t.Deadline,
                CreatedAt: t.CreatedAt,
                UpdatedAt: t.UpdatedAt)).ToList(),
            Page: result.PageNumber,
            TotalPages: result.TotalPages,
            Count: result.TotalCount,
            HasPreviousPage: result.HasPreviousPage,
            HasNextPage: result.HasNextPage);

        return Ok(response);
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportTaskItems([FromQuery] ExportTaskItemsRequest request)
    {
        if (!TryGetUserId(User, out var userId))
        {
            return Unauthorized("Missing or invalid user id claim.");
        }

        var query = new ExportTaskItemsQuery(
            UserId: userId,
            Search: request.Search,
            Statuses: request.Statuses,
            OrderBy: request.OrderBy,
            SortDirection: request.OrderByDirection,
            Format: request.Format);

        var result = await exportTaskItemsHandler.Handle(query, HttpContext.RequestAborted);

        return File(result.Content, result.MimeType, "my_tasks");
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetTaskItemResponse>> FindTaskItem(Guid id)
    {
        if (!TryGetUserId(User, out var userId))
        {
            return Unauthorized("Missing or invalid user id claim.");
        }

        var query = new FindTaskItemQuery(TaskItemId: id, UserId: userId);
        var taskItem = await findTaskItemHandler.Handle(query, HttpContext.RequestAborted);
        if (taskItem is null)
        {
            return NotFound();
        }

        return Ok(new GetTaskItemResponse(
            Id: taskItem.Id,
            Title: taskItem.Title,
            Description: taskItem.Description,
            Status: taskItem.Status,
            Deadline: taskItem.Deadline,
            CreatedAt: taskItem.CreatedAt,
            UpdatedAt: taskItem.UpdatedAt));
    }
    
    [HttpPost]
    public async Task<ActionResult<AddTaskItemResponse>> AddTaskItem(AddTaskItemRequest request)
    {
        if (!TryGetUserId(User, out var userId))
        {
            return Unauthorized("Missing or invalid user id claim.");
        }

        var command = new AddTaskItemCommand(
            Title: request.Title,
            Description: request.Description,
            Status: request.Status,
            Deadline: request.Deadline,
            UserId: userId);

        var taskItemId = await addTaskItemHandler.Handle(command, HttpContext.RequestAborted);

        return Ok(new AddTaskItemResponse(Id: taskItemId));
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskItemResponse>> UpdateTaskItem(Guid id, UpdateTaskItemRequest request)
    {
        if (!TryGetUserId(User, out var userId))
        {
            return Unauthorized("Missing or invalid user id claim.");
        }

        var command = new UpdateTaskItemCommand(
            TaskItemId: id,
            Title: request.Title,
            Description: request.Description,
            Status: request.Status,
            Deadline: request.Deadline,
            UserId: userId);

        var taskItem = await updateTaskItemHandler.Handle(command, HttpContext.RequestAborted);

        return Ok(new TaskItemResponse(
            Id: taskItem.Id,
            Description: taskItem.Description,
            Title: taskItem.Title,
            Status: taskItem.Status,
            Deadline: taskItem.Deadline,
            CreatedAt: taskItem.CreatedAt,
            UpdatedAt: taskItem.UpdatedAt));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<TaskItemResponse>> DeleteTaskItem(Guid id)
    {
        if (!TryGetUserId(User, out var userId))
        {
            return Unauthorized("Missing or invalid user id claim.");
        }

        var command = new DeleteTaskItemCommand(TaskItemId: id, UserId: userId);
        await deleteTaskItemHandler.Handle(command, HttpContext.RequestAborted);

        return NoContent();
    }

    private static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst(JwtRegisteredClaimNames.Sub);
        
        return Guid.TryParse(claim?.Value, out userId);
    }
}
using Application.Abstractions;
using Application.Authentication.Commands.Register;
using Application.Authentication.DTOs;
using Application.Authentication.Events.UserRegistered;
using Application.Authentication.Queries.Login;
using Application.Common;
using Application.Common.Decorators;
using Application.TaskItems.Commands.AddTaskItem;
using Application.TaskItems.Commands.DeleteTaskItem;
using Application.TaskItems.Commands.UpdateTaskItem;
using Application.TaskItems.DTOs;
using Application.TaskItems.Queries.ExportTaskItems;
using Application.TaskItems.Queries.FindTaskItem;
using Application.TaskItems.Queries.GetTaskItems;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<RegisterCommand, Guid>, RegisterCommandHandler>();
        services.AddScoped<ICommandHandler<AddTaskItemCommand, Guid>, AddTaskItemCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTaskItemCommand, TaskItem>, UpdateTaskItemCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteTaskItemCommand, Unit>, DeleteTaskItemCommandHandler>();
        services.AddScoped<IQueryHandler<LoginQuery, LoginResult>, LoginQueryHandler>();
        services.AddScoped<IQueryHandler<FindTaskItemQuery, TaskItem?>, FindTaskItemQueryHandler>();
        services.AddScoped<IQueryHandler<GetTaskItemsQuery, PaginatedList<TaskItem>>, GetTaskItemsQueryHandler>();
        services.AddScoped<IQueryHandler<ExportTaskItemsQuery, ExportedTaskItemsDto>, ExportTaskItemsQueryHandler>();

        services.AddScoped<SendEmailOnUserRegisteredEventHandler>();
        services.AddScoped<SendSmsOnUserRegisteredEventHandler>();

        services.Decorate(typeof(ICommandHandler<,>), typeof(TransactionalCommandHandler<,>));

        return services;
    }
}
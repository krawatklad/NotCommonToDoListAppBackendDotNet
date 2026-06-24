using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Common.Exceptions;

internal sealed class ProblemDetailsFactory : IProblemDetailsFactory
{
    private static readonly Dictionary<Type, Func<Exception, ProblemDetails>> Map
        = new()
        {
            {
                typeof(ValidationException),
                ex => new ProblemDetails
                {
                    Title = ex.GetType().Name,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ex.Message
                }
            },
            {
                typeof(ForbiddenException),
                ex => new ProblemDetails
                {
                    Title = ex.GetType().Name,
                    Status = StatusCodes.Status403Forbidden,
                    Detail = ex.Message
                }
            },
            {
                typeof(NotFoundException),
                ex => new ProblemDetails
                {
                    Title = ex.GetType().Name,
                    Status = StatusCodes.Status404NotFound,
                    Detail = ex.Message
                }
            }
        };

    public ProblemDetails Create(Exception exception)
    {
        if (Map.TryGetValue(exception.GetType(), out var factory))
        {
            return factory(exception);
        }

        return new ProblemDetails
        {
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message,
            Type = exception.GetType().Name
        };
    }
}
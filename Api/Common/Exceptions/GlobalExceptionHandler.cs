using Microsoft.AspNetCore.Diagnostics;

namespace Api.Common.Exceptions;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService, 
    IProblemDetailsFactory problemDetailsFactory, 
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "An unhandled exception occurred while processing the request. RequestId: {HttpContextTraceIdentifier}",
            httpContext.TraceIdentifier);

        var problemDetails = problemDetailsFactory.Create(exception);
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}
using Microsoft.AspNetCore.Mvc;

namespace Api.Common.Exceptions;

internal interface IProblemDetailsFactory
{
    ProblemDetails Create(Exception exception);
}
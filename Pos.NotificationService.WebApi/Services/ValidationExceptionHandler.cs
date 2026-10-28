using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pos.NotificationService.Application.Exceptions;
namespace Pos.NotificationService.WebApi.Services;
public class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validation) return false;
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = 400, Title = "Validation failed.", Detail = exception.Message, Extensions = { ["errors"] = validation.Errors }
        }, cancellationToken);
        return true;
    }
}


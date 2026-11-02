using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.NotificationService.Application.Features.NotificationTemplates.Commands.CreateCommand;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.WebApi.Policies;

namespace Pos.NotificationService.WebApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notification-templates")]
[Authorize(Policy = AppPolicies.ManageTemplates)]
public class NotificationTemplatesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Response<Guid>>> Create(
        [FromBody] CreateNotificationTemplateCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return BadRequest(new Response<Guid> { Succeeded = false, Errors = result.Errors });

        return Ok(new Response<Guid>(result.Value));
    }
}


using Pos.NotificationService.Application.Features.NotificationTemplates.DTOs;
using Pos.NotificationService.Application.Features.NotificationTemplates.Commands.UpdateCommand;
using Pos.NotificationService.Application.Features.NotificationTemplates.Commands.SetActiveStatusCommand;
using Pos.NotificationService.Application.Features.NotificationTemplates.Queries.GetNotificationTemplatesQuery;
using Pos.NotificationService.Application.Features.NotificationTemplates.Queries.GetNotificationTemplateByIdQuery;
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

    [HttpGet]
    public async Task<ActionResult<PagedResponse<IEnumerable<NotificationTemplateDto>>>> GetTemplates(
        [FromQuery] GetNotificationTemplatesQuery query, CancellationToken cancellationToken)
        => Ok(await mediator.Send(query, cancellationToken));

    [HttpGet("{templateId:guid}")]
    public async Task<ActionResult<Response<NotificationTemplateDto>>> GetById(
        Guid templateId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetNotificationTemplateByIdQuery { TemplateId = templateId }, cancellationToken);
        if (result.IsFailure)
            return NotFound(new Response<NotificationTemplateDto> { Succeeded = false, Errors = result.Errors });
        return Ok(new Response<NotificationTemplateDto>(result.Value!));
    }

    [HttpPut("{templateId:guid}")]
    public async Task<ActionResult<Response<Guid>>> Update(Guid templateId,
        [FromBody] UpdateNotificationTemplateCommand command, CancellationToken cancellationToken)
    {
        if (command.TemplateId != Guid.Empty && command.TemplateId != templateId)
            return BadRequest(new Response<Guid> { Errors = new() { "Template ID does not match the route." } });
        command.TemplateId = templateId;
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return BadRequest(new Response<Guid> { Succeeded = false, Errors = result.Errors });
        return Ok(new Response<Guid>(result.Value));
    }

    [HttpPatch("{templateId:guid}/active-status")]
    public async Task<ActionResult<Response<Guid>>> SetActiveStatus(Guid templateId,
        [FromBody] SetNotificationTemplateActiveStatusCommand command, CancellationToken cancellationToken)
    {
        if (command.TemplateId != Guid.Empty && command.TemplateId != templateId)
            return BadRequest(new Response<Guid> { Errors = new() { "Template ID does not match the route." } });
        command.TemplateId = templateId;
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return BadRequest(new Response<Guid> { Succeeded = false, Errors = result.Errors });
        return Ok(new Response<Guid>(result.Value));
    }
}


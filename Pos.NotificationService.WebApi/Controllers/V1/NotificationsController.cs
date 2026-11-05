using Pos.NotificationService.Application.Features.Notifications.Commands.MarkNotificationAsReadCommand;
using Pos.NotificationService.Application.Features.Notifications.Commands.MarkAllNotificationsAsReadCommand;
using Pos.NotificationService.Application.Features.Notifications.DTOs;
using Pos.NotificationService.Application.Features.Notifications.Queries.GetMyNotificationByIdQuery;
using Pos.NotificationService.Application.Features.Notifications.Queries.GetUnreadNotificationCountQuery;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.NotificationService.Application.Features.Notifications.Queries.GetMyNotificationsQuery;
using Pos.NotificationService.Application.Wrappers;
using Pos.NotificationService.WebApi.Policies;

namespace Pos.NotificationService.WebApi.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/notifications")]
    [Authorize(Policy = AppPolicies.AccessOwnNotifications)]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<IEnumerable<NotificationDetailsDto>>>> GetMyNotifications(
            [FromQuery] GetMyNotificationsQuery query,
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(query, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{notificationId:guid}")]
        public async Task<ActionResult<Response<NotificationDetailsDto>>> GetMyNotificationById(
            [FromRoute] Guid notificationId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetMyNotificationByIdQuery { NotificationId = notificationId }, cancellationToken);

            if (result.IsFailure)
                return NotFound(new Response<NotificationDetailsDto>
                {
                    Succeeded = false,
                    Errors = result.Errors
                });

            return Ok(new Response<NotificationDetailsDto>(result.Value!));
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<Response<int>>> GetUnreadNotificationCount(
            CancellationToken cancellationToken)
        {
            var count = await _mediator.Send(new GetUnreadNotificationCountQuery(), cancellationToken);
            return Ok(new Response<int>(count));
        }

        [HttpPatch("{notificationId:guid}/read")]
        public async Task<ActionResult<Response<Guid>>> MarkNotificationAsRead(
            [FromRoute] Guid notificationId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new MarkNotificationAsReadCommand { NotificationId = notificationId }, cancellationToken);

            if (result.IsFailure)
                return NotFound(new Response<Guid> { Succeeded = false, Errors = result.Errors });

            return Ok(new Response<Guid>(result.Value));
        }

        [HttpPatch("read-all")]
        public async Task<ActionResult<Response<int>>> MarkAllNotificationsAsRead(
            CancellationToken cancellationToken)
        {
            var count = await _mediator.Send(new MarkAllNotificationsAsReadCommand(), cancellationToken);
            return Ok(new Response<int>(count));
        }
    }
}
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.NotificationService.Application.Wrappers;

namespace Pos.NotificationService.WebApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/status")]
public class StatusController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<Response<string>> Get()
        => Ok(new Response<string>("NotificationService is running."));
}

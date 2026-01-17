using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace SimpleKsef.Infrastructure.Web;

[ApiController]
[ApiVersion(1.0)]
[Consumes("application/json")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status406NotAcceptable)] // client asked for non-json (Accept)
[ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)] // client sent non-json (Content-Type)
[Route("api/v{apiVersion:apiVersion}/[controller]")]
public abstract class BaseApiController : ControllerBase
{
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Skeptical.Beavers.Backend.Controllers
{
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public abstract class BaseController : Controller
    {

    }
}
using Microsoft.AspNetCore.Mvc;

namespace Skeptical.Beavers.Backend.Controllers
{
    public sealed class ChallengeController : BaseController
    {
        [HttpPost("/challenge")]
        public IActionResult ConfirmAppIdentity()
        {

        }
    }
}
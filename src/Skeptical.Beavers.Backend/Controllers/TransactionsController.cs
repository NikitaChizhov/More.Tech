using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skeptical.Beavers.Backend.Model;
using Skeptical.Beavers.Backend.Services;
using Skeptical.Beavers.Backend.Utils;

namespace Skeptical.Beavers.Backend.Controllers
{
    [Authorize]
    public sealed class TransactionsController : BaseController
    {
        private readonly IAppsService _apps;

        public TransactionsController(IAppsService apps)
        {
            _apps = apps;
        }

        [HttpPost(Routes.Transaction, Name = nameof(ProcessTransaction))]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [Consumes(HttpContentTypes.MultipartFormData, HttpContentTypes.ApplicationJson)]
        public IActionResult ProcessTransaction([FromForm, FromBody] TransactionData data)
        {
            var userName = User?.Identity?.Name;
            if (userName == null)
            {
                return NotFound();
            }

            if (!Request.Headers.ContainsKey("App-Auth") || !_apps.IsThisUsersAppKey(userName, Request.Headers["App-Auth"]))
            {
                return NotFound(); // endpoint will be masked, so Unauthorized() is an additional info we don't want to share
            }

            return Accepted();
        }

    }
}
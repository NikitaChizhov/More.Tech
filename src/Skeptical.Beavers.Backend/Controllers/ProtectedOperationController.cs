using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Skeptical.Beavers.Backend.Model;

namespace Skeptical.Beavers.Backend.Controllers
{
    [Authorize]
    public sealed class ProtectedOperationController : BaseController
    {
        [HttpPost("transaction", Name = nameof(ProcessTransaction))]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [Consumes(HttpContentTypes.MultipartFormData, HttpContentTypes.ApplicationJson)]
        public IActionResult ProcessTransaction([FromForm, FromBody] TransactionData data)
        {
            return Accepted();
        }

    }
}
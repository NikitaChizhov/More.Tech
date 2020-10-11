using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skeptical.Beavers.Backend.Challenges;
using Skeptical.Beavers.Backend.Model;

namespace Skeptical.Beavers.Backend.Controllers
{
    [Authorize]
    public sealed class ChallengeController : BaseController
    {
        private readonly IChallengeRepository _challengeRepository;

        private readonly ILogger<ChallengeController> _logger;

        public ChallengeController(IChallengeRepository challengeRepository, ILogger<ChallengeController> logger)
        {
            _challengeRepository = challengeRepository;
            _logger = logger;
        }

        [HttpPost(Routes.Challenge, Name = nameof(ConfirmAppIdentity))]
        public IActionResult ConfirmAppIdentity([FromBody] ChallengeRequest data, [FromRoute] Guid appId)
        {
            var userName = User?.Identity?.Name;
            if (userName == null || !_challengeRepository.IsPassed(userName, appId, data))
            {
                return Unauthorized();
            }

            return Ok();
        }
    }
}
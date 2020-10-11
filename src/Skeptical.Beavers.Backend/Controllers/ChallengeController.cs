using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skeptical.Beavers.Backend.Challenges;
using Skeptical.Beavers.Backend.Model;
using Skeptical.Beavers.Backend.Services;

namespace Skeptical.Beavers.Backend.Controllers
{
    [Authorize]
    public sealed class ChallengeController : BaseController
    {
        private readonly IChallengeRepository _challengeRepository;

        private readonly IAppsService _apps;

        private readonly ILogger<ChallengeController> _logger;

        public ChallengeController(IChallengeRepository challengeRepository, ILogger<ChallengeController> logger, IAppsService apps)
        {
            _challengeRepository = challengeRepository;
            _logger = logger;
            _apps = apps;
        }

        [HttpPost(Routes.Challenge, Name = nameof(ConfirmAppIdentity))]
        public IActionResult ConfirmAppIdentity([FromBody] ChallengeRequest data, [FromRoute] Guid appId)
        {
            var userName = User?.Identity?.Name;
            if (userName == null || !_challengeRepository.IsPassed(userName, appId, data))
            {
                return NotFound(); // endpoint will be masked, so Unauthorized() is an additional info we don't want to share
            }

            return Ok(new ChallengeResponse{ AppAuth = _apps.GenerateAndRememberAppKey(userName) });
        }
    }
}
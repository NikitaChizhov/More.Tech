using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Skeptical.Beavers.Backend.Challenges;
using Skeptical.Beavers.Backend.Configurations;
using Skeptical.Beavers.Backend.Model;
using Skeptical.Beavers.Backend.Obfuscation;
using Skeptical.Beavers.Backend.Services;

namespace Skeptical.Beavers.Backend.Controllers
{
    [Authorize]
    public sealed class SecretAppController : BaseController
    {
        private readonly ILogger<SecretAppController> _logger;

        private readonly NpmConfig _npmConfig;

        private readonly IAppsService _apps;

        private readonly IChallengeRepository _challengeRepository;

        private readonly ObfuscatedEndpointsRepository _obfuscatedEndpoints;

        public SecretAppController(ILogger<SecretAppController> logger, NpmConfig npmConfig, IAppsService apps, IChallengeRepository challengeRepository, ObfuscatedEndpointsRepository obfuscatedEndpoints)
        {
            _logger = logger;
            _npmConfig = npmConfig;
            _apps = apps;
            _challengeRepository = challengeRepository;
            _obfuscatedEndpoints = obfuscatedEndpoints;
        }

        [HttpGet("app")]
        [Produces("text/html")]
        public async Task<IActionResult> Get()
        {
            var userName = User?.Identity?.Name;
            if(userName == null) return Unauthorized();

            var appId= Guid.NewGuid();
            var challengeKey = _obfuscatedEndpoints.StoreEndpoint($"/challenge/{appId}");
            var challengeData = new ChallengeRequest{ Data = "random data" };
            var challenge = new FixedDataSenderChallenge(challengeData, $"/{challengeKey}");

            BuildApp(challenge);
            _apps.PrepareAppToBeServed(appId, userName, "/front/dist");
            _challengeRepository.SaveChallenge(challenge, userName, appId);

            var indexHtml = await _apps.GetHtmlAsync(appId);

            return Content(ObfuscateEndpoints(indexHtml), "text/html");
        }

        [HttpGet("app/{id:guid}/bundle.js")]
        [Produces("application/javascript")]
        [AllowAnonymous]
        public async Task<IActionResult> GetJs([FromRoute(Name = "id")] Guid appId)
        {
            var js = await _apps.GetJsBundleAsync(appId);
            return Content(ObfuscateEndpoints(js), "application/javascript");
        }

        private void BuildApp(ISingleCallChallenge challenge)
        {
            // place the challenge
            var templatePath = System.IO.Path.Combine(_npmConfig.Prefix, "templates/HomeTemplate.js");
            var template = System.IO.File.ReadAllText(templatePath);
            var targetPath = System.IO.Path.Combine(_npmConfig.Prefix, "src/Home.js");
            System.IO.File.WriteAllText(targetPath, template
                .Replace("//_}*1*{_", challenge.ChallengeFunction));

            // npm build
            using var process = new Process();
            process.StartInfo.FileName = "npm";
            process.StartInfo.Arguments = $"run --prefix \"{_npmConfig.Prefix}\" build-silent";

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            // wait until npm build is done, throw if not done in 30 seconds
            if (!process.WaitForExit(1000 * 30))
            {
                throw new TimeoutException("Npm took too long to run");
            }

            _logger.LogError(process.StandardError.ReadToEnd());
        }

        private string ObfuscateEndpoints(in string str)
        {
            var transactionKey = _obfuscatedEndpoints.StoreEndpoint(Routes.Transaction);
            return str
                .Replace(Routes.Transaction, $"/{transactionKey}");
        }
    }
}
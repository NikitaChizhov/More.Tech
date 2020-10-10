using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skeptical.Beavers.Backend.Configurations;
using Skeptical.Beavers.Backend.Services;

namespace Skeptical.Beavers.Backend.Controllers
{
    [Authorize]
    public sealed class SecretAppController : BaseController
    {
        private readonly ILogger<SecretAppController> _logger;

        private readonly NpmConfig _npmConfig;

        private readonly IAppsService _apps;

        public SecretAppController(ILogger<SecretAppController> logger, NpmConfig npmConfig, IAppsService apps)
        {
            _logger = logger;
            _npmConfig = npmConfig;
            _apps = apps;
        }

        [HttpGet("app")]
        [Produces("text/html")]
        public async Task<IActionResult> Get()
        {
            var userName = User?.Identity?.Name;
            if(userName == null) return Unauthorized();

            _logger.LogInformation($"Building app for {userName}");
            BuildApp();
            var appId = _apps.NewApp(userName, "/front/dist");
            var appHtml = await _apps.GetHtmlAsync(appId);
            _logger.LogInformation(appHtml);

            return Content(appHtml, "text/html");
        }

        [HttpGet("app/{id:guid}/bundle.js")]
        [Produces("application/javascript")]
        [AllowAnonymous]
        public async Task<IActionResult> GetJs([FromRoute(Name = "id")] Guid appId)
        {
            return Content(await _apps.GetJsBundleAsync(appId), "application/javascript");
        }

        private void BuildApp()
        {
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
    }
}
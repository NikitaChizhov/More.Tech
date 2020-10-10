using System;
using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Skeptical.Beavers.Backend.JsonWebTokens;
using Skeptical.Beavers.Backend.Model;
using Skeptical.Beavers.Backend.Services;

namespace Skeptical.Beavers.Backend.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class LoginController : BaseController
    {
        private readonly ILogger<LoginController> _logger;

        private readonly IJwtAuthManager _jwtAuthManager;

        private readonly IUserService _userService;

        public LoginController(IJwtAuthManager jwtAuthManager, ILogger<LoginController> logger, IUserService userService)
        {
            _jwtAuthManager = jwtAuthManager;
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("login", Name = nameof(Login))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [Consumes(HttpContentTypes.MultipartFormData, HttpContentTypes.ApplicationJson)]
        public IActionResult Login([FromForm, FromBody] LoginRequest request)
        {
            if (!_userService.IsValidUserCredentials(request.UserName, request.Password))
            {
                return Unauthorized();
            }

            var claims = new[] { new Claim(ClaimTypes.Name, request.UserName) };
            var token = _jwtAuthManager.GenerateToken(request.UserName, claims, DateTime.UtcNow);
            _logger.LogInformation($"User [{request.UserName}] logged in the system.");

            return Ok(new LoginResponse
            {
                UserName = request.UserName,
                AccessToken = token
            });
        }
    }
}
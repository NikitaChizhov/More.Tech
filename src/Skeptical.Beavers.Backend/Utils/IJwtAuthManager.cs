using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Skeptical.Beavers.Backend.Utils
{
    public interface IJwtAuthManager
    {
        string GenerateToken(string username, Claim[] claims, DateTime now);

        (ClaimsPrincipal, JwtSecurityToken) DecodeJwtToken(string token);
    }
}
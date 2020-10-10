namespace Skeptical.Beavers.Backend.JsonWebTokens
{
    internal sealed class JwtConfig
    {
        public string Secret { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int AccessTokenExpiration { get; set; }
    }
}
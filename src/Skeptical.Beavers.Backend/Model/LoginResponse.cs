namespace Skeptical.Beavers.Backend.Model
{
    public sealed class LoginResponse
    {
        public string UserName { get; set; }

        public string AccessToken { get; set; }
    }
}
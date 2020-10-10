namespace Skeptical.Beavers.Backend.Services
{
    public interface IUserService
    {
        bool IsAnExistingUser(string userName);

        bool IsValidUserCredentials(string userName, string password);
    }
}
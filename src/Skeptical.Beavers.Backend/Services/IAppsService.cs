using System;
using System.Threading.Tasks;

namespace Skeptical.Beavers.Backend.Services
{
    public interface IAppsService
    {
        void PrepareAppToBeServed(Guid appId, string userName, string distDir);

        Task<string> GetHtmlAsync(Guid appId);

        Task<string> GetJsBundleAsync(Guid appId);

        string GenerateAndRememberAppKey(string userName);

        bool IsThisUsersAppKey(string userName, string appKey);
    }
}
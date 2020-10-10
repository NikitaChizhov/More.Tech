using System;
using System.Threading.Tasks;

namespace Skeptical.Beavers.Backend.Services
{
    public interface IAppsService
    {
        Guid NewApp(string userName, string distDir);

        Task<string> GetHtmlAsync(Guid appId);

        Task<string> GetJsBundleAsync(Guid appId);

        bool TryGetAppId(string userName, out Guid appId);

        string GetAppDir(Guid appId);
    }
}
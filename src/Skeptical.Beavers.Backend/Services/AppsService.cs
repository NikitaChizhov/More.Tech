using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skeptical.Beavers.Backend.Services
{
    internal sealed class AppsService : IAppsService
    {
        private readonly Dictionary<string, Guid> _ids = new Dictionary<string, Guid>();

        private static string GetAppDir(Guid appId) => $"/apps/{appId}";

        /// <inheritdoc />
        public void PrepareAppToBeServed(Guid appId, string userName, string distDir)
        {
            System.IO.Directory.CreateDirectory(GetAppDir(appId));

            if (_ids.TryGetValue(userName, out var oldValue))
            {
                System.IO.Directory.Delete(GetAppDir(oldValue), true);
            }

            _ids[userName] = appId;

            var appDir = GetAppDir(appId);

            // move
            foreach (var sourceFile in System.IO.Directory.GetFiles(distDir))
            {
                var destFileName = System.IO.Path.Combine(appDir, System.IO.Path.GetFileName(sourceFile));
                System.IO.File.Copy(sourceFile, destFileName, true);
            }
        }

        /// <inheritdoc />
        public async Task<string> GetHtmlAsync(Guid appId)
        {
            var fileAsString = await System.IO.File.ReadAllTextAsync(System.IO.Path.Combine(GetAppDir(appId), "index.html"));
            return fileAsString.Replace("bundle.js", $"app/{appId}/bundle.js");
        }

        /// <inheritdoc />
        public Task<string> GetJsBundleAsync(Guid appId)
        {
            var path = System.IO.Path.Combine(GetAppDir(appId), "bundle.js");
            return System.IO.File.ReadAllTextAsync(path);
        }
    }
}
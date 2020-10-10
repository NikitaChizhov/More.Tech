using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skeptical.Beavers.Backend.Services
{
    internal sealed class AppsService : IAppsService
    {
        private readonly Dictionary<string, Guid> _ids = new Dictionary<string, Guid>();

        /// <inheritdoc />
        public Guid NewApp(string userName, string distDir)
        {
            var appId = Guid.NewGuid();

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

            return appId;
        }

        /// <inheritdoc />
        public async Task<string> GetHtmlAsync(Guid appId)
        {
            var fileAsString = await System.IO.File.ReadAllTextAsync(System.IO.Path.Combine(GetAppDir(appId), "index.html"));
            return fileAsString.Replace("bundle.js", $"app/{appId}/bundle.js");
        }

        /// <inheritdoc />
        public Task<string> GetJsBundleAsync(Guid appId) => System.IO.File.ReadAllTextAsync(System.IO.Path.Combine(GetAppDir(appId), "bundle.js"));

        /// <inheritdoc />
        public bool TryGetAppId(string userName, out Guid appId) => _ids.TryGetValue(userName, out appId);

        /// <inheritdoc />
        public string GetAppDir(Guid appId) => $"/apps/{appId}";
    }
}
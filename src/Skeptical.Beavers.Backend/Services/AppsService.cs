using System;
using System.Collections.Generic;

namespace Skeptical.Beavers.Backend.Services
{
    internal sealed class AppsService : IAppsService
    {
        private readonly Dictionary<string, Guid> _ids = new Dictionary<string, Guid>();

        /// <inheritdoc />
        public Guid NewAppId(string userName)
        {
            var appId = Guid.NewGuid();

            System.IO.Directory.CreateDirectory(GetAppDir(appId));

            if (_ids.TryGetValue(userName, out var oldValue))
            {
                System.IO.Directory.Delete(GetAppDir(appId), true);
            }

            _ids[userName] = appId;

            return appId;
        }

        /// <inheritdoc />
        public bool TryGetAppId(string userName, out Guid appId) => _ids.TryGetValue(userName, out appId);

        /// <inheritdoc />
        public string GetAppDir(Guid appId) => $"/apps/{appId}";
    }
}
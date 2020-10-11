using System;
using System.Collections.Concurrent;

namespace Skeptical.Beavers.Backend.Obfuscation
{
    public sealed class ObfuscatedEndpointsRepository
    {
        private readonly ConcurrentDictionary<Guid, string> _map = new ConcurrentDictionary<Guid, string>();

        public Guid StoreEndpoint(string endpoint)
        {
            var key = Guid.NewGuid();
            _map.AddOrUpdate(key, endpoint, (_, __) => endpoint);
            return key;
        }

        public bool TryGet(Guid key, out string type) => _map.TryGetValue(key, out type);
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Skeptical.Beavers.Backend.Obfuscation
{
    public sealed class EndpointDeobfuscationMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ObfuscatedEndpointsRepository _repository;

        private readonly ILogger<EndpointDeobfuscationMiddleware> _logger;

        public EndpointDeobfuscationMiddleware(RequestDelegate next, ObfuscatedEndpointsRepository repository, ILogger<EndpointDeobfuscationMiddleware> logger)
        {
            _next = next;
            _repository = repository;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (Guid.TryParse(context.Request.Path.Value.TrimStart('/'), out var guid) && _repository.TryGet(guid, out var endpoint))
            {
                var mappedPath = new PathString(endpoint);
                _logger.LogInformation($"Swapping {context.Request.Path} to {mappedPath}");
                context.Request.Path = mappedPath;
            }

            await _next(context);
        }
    }
}
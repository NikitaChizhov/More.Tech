using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Skeptical.Beavers.Backend.Services
{
    internal sealed class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;

        private readonly ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>(new Dictionary<string, string>
        {
            { "admin", "admin" },
            { "user1", "user1" }
        });

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public bool IsAnExistingUser(string userName)
        {
            return _users.ContainsKey(userName);
        }

        /// <inheritdoc />
        public bool IsValidUserCredentials(string userName, string password)
        {
            _logger.LogInformation($"Validating user [{userName}]");
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            return _users.TryGetValue(userName, out var p) && p == password;
        }
    }
}
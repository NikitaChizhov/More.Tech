using System;

namespace Skeptical.Beavers.Backend.Challenges
{
    internal struct UserAppPair : IEquatable<UserAppPair>
    {
        public UserAppPair(string userName, Guid appId)
        {
            UserName = userName;
            AppId = appId;
        }

        public Guid AppId { get; }

        public string UserName { get; }

        /// <inheritdoc />
        public bool Equals(UserAppPair other) => AppId.Equals(other.AppId) && UserName == other.UserName;

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is UserAppPair other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(AppId, UserName);

        public static bool operator ==(UserAppPair left, UserAppPair right) => left.Equals(right);

        public static bool operator !=(UserAppPair left, UserAppPair right) => !left.Equals(right);
    }
}
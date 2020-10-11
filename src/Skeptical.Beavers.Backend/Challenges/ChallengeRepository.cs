using System;
using System.Collections.Generic;
using Skeptical.Beavers.Backend.Model;

namespace Skeptical.Beavers.Backend.Challenges
{
    internal sealed class ChallengeRepository : IChallengeRepository
    {
        private readonly Dictionary<UserAppPair, ISingleCallChallenge> _challenges = new Dictionary<UserAppPair, ISingleCallChallenge>();

        public void SaveChallenge(ISingleCallChallenge challenge, string userName, Guid appId) =>
            _challenges[new UserAppPair(userName, appId)] = challenge;

        public bool IsPassed(string userName, Guid appId, ChallengeRequest receivedRequest) =>
            _challenges.TryGetValue(new UserAppPair(userName, appId), out var challenge) &&
            challenge.IsPassed(receivedRequest);
    }
}
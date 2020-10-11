using System;
using Skeptical.Beavers.Backend.Model;

namespace Skeptical.Beavers.Backend.Challenges
{
    public interface IChallengeRepository
    {
        void SaveChallenge(ISingleCallChallenge challenge, string userName, Guid appId);

        bool IsPassed(string userName, Guid appId, ChallengeRequest receivedRequest);
    }
}
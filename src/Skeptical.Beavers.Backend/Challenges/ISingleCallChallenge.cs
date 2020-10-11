using Skeptical.Beavers.Backend.Model;

namespace Skeptical.Beavers.Backend.Challenges
{
    public interface ISingleCallChallenge
    {
        string ChallengeFunction { get; }

        bool IsPassed(ChallengeRequest receivedRequest);
    }
}
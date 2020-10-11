using Skeptical.Beavers.Backend.Model;

namespace Skeptical.Beavers.Backend.Challenges
{
    public interface ISingleCallChallenge
    {
        string ChallengeFunction { get; }

        string FunctionCall { get; }

        bool IsPassed(ChallengeRequest receivedRequest);
    }
}
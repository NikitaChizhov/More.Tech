using Newtonsoft.Json;
using Skeptical.Beavers.Backend.Model;

namespace Skeptical.Beavers.Backend.Challenges
{
    internal sealed class FixedDataSenderChallenge : ISingleCallChallenge
    {
        private readonly ChallengeRequest _challengeRequest;

        public FixedDataSenderChallenge(ChallengeRequest challengeRequest, string endpoint)
        {
            _challengeRequest = challengeRequest;
            ChallengeFunction = $@"function identify(data) {{ return fetch('{endpoint}', {{ method: 'POST', headers: {{ 'Authorization': `${{localStorage.getItem('token')}}`, 'Content-Type': 'Application/json' }}, body: JSON.stringify(data) }}) }};";
        }

        public string ChallengeFunction { get; }

        public string FunctionCall => $"identify({JsonConvert.SerializeObject(_challengeRequest)}).then(res => status = 'SUCCESS');";

        public bool IsPassed(ChallengeRequest receivedRequest) => _challengeRequest.Data == receivedRequest.Data;
    }
}
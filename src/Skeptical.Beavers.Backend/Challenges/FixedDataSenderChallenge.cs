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
            ChallengeFunction = $@"fetch('{endpoint}', {{method: 'POST', headers: {{ 'Authorization': `${{localStorage.getItem('token')}}`, 'Content-Type': 'Application/json' }}, body: '{JsonConvert.SerializeObject(_challengeRequest)}' }}).then(res => {{setState(res.ok); return res.json();}}).then(res => key = res['appAuth'])";
        }

        public string ChallengeFunction { get; }

        public bool IsPassed(ChallengeRequest receivedRequest) => _challengeRequest.Data == receivedRequest.Data;
    }
}
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver
{
    public class LeaderElectorService : ILeaderElectorService
    {
        private readonly HttpClient _httpClient;
        private readonly Uri _leaderElectorUri;

        public LeaderElectorService(HttpClient httpClient,
            Uri leaderElectorUri)
        {
            _httpClient = httpClient;
            _leaderElectorUri = leaderElectorUri;
        }

        public async Task<bool> CanProceedAsLeader(Guid id)
        {
            var response = await _httpClient.GetAsync(_leaderElectorUri + "ProceedAsLeader?caller=" + id);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            var result = bool.Parse(responseBody);

            return result;
        }
    }
}

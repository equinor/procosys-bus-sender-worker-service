using System;
using System.Net.Http;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Microsoft.Extensions.Logging;

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
            HttpResponseMessage response = await _httpClient.GetAsync(_leaderElectorUri + "ProceedAsLeader?caller=" + id);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            //var responseBody = await _httpClient.GetStringAsync(_leaderElectorUri + "/ProceedAsLeader?caller=" + id);
            var result = bool.Parse(responseBody);

            return result;
        }
    }
}

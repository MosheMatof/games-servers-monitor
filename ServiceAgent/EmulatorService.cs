using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceAgent.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAgent
{
    public class EmulatorService : IEmulatorService
    {
        private readonly ILogger<EmulatorService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public EmulatorService(ILogger<EmulatorService> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _baseUrl = "http://localhost:5770/api/emulator";
        }

        //public Task StartAsync(CancellationToken cancellationToken)
        //{
        //    Init();
        //    return Task.CompletedTask;
        //}

        //public Task StopAsync(CancellationToken cancellationToken)
        //{
        //    return Task.CompletedTask;
        //}

        //public void Dispose()
        //{
        //    _httpClient?.Dispose();
        //}

        public async Task<bool> StartEmulator(int numOfGames = 20, int numOfServers = 10, int interval = 10000)
        {            
            var url = $"{_baseUrl}/init";

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(new { NumOfGames = numOfGames, NumOfServers = numOfServers, Interval = interval }), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to initialize emulator. Status code: {response.StatusCode} message: {message}");
                    return false;
                }
                else
                {
                    _logger.LogInformation("Emulator initialized successfully.");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the emulator.");
                return false;
            }
        }
        public async void StopEmulator()
        {
            _logger.LogInformation("stop emulator has requested");
            var url = $"{_baseUrl}/stop";
            var response = await _httpClient.PostAsync(url, null).ConfigureAwait(false);
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _logger.LogInformation($"the emulator response: {message}");
        }
    }
}

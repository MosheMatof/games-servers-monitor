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

        public async Task<bool> StartEmulatorAsync(int numOfGames = 20, int numOfServers = 10, int interval = 10000)
        {
            _logger.LogInformation("start emulator has requested");
            var url = $"{_baseUrl}/init";

            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(new { NumOfGames = numOfGames, NumOfServers = numOfServers, Interval = interval }), Encoding.UTF8, "application/json");
                var response = _httpClient.PostAsync(url, content).Result;
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
        public async Task<bool> ResumeEmulatorAsync()
        {
            _logger.LogInformation("resume emulator has requested");
            var url = $"{_baseUrl}/resume";
            var response = await _httpClient.PostAsync(url, null).ConfigureAwait(false);
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _logger.LogInformation($"the emulator server response: {message}");
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> StopEmulatorAsync()
        {
            _logger.LogInformation("stop emulator has requested");
            var url = $"{_baseUrl}/stop";
            var response = await _httpClient.PostAsync(url, null).ConfigureAwait(false);
            var message = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            _logger.LogInformation($"the emulator server response: {message}");
            return response.IsSuccessStatusCode;
        }
    }
}

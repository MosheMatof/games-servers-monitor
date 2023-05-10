using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BL.BE;
using ServiceAgent.Contracts;

namespace ServiceAgent
{
    public class GameService : IGameService
    {
        private readonly HttpClient _httpClient;

        public GameService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async IAsyncEnumerable<BEGame> GetGameInfoAsync(List<int> gameIds)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/games/info")
            {
                Content = new StringContent(JsonSerializer.Serialize(gameIds), Encoding.UTF8, "application/json")
            };

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(line) && line.StartsWith("data:"))
                {
                    // Remove the "data:" prefix and trim the line
                    var messageJson = line.Substring("data:".Length).Trim();

                    var message = JsonSerializer.Deserialize<SSEMessage>(messageJson);
                    if (message.Event == "game")
                    {
                        var game = JsonSerializer.Deserialize<BEGame>(message.Data);
                        yield return game;
                    }
                    else if (message.Event == "error")
                    {
                        throw new Exception(message.Data);
                    }
                    else
                    {
                        throw new Exception($"Unknown message event");
                    }
                }
            }
        }


        public async Task<BEGame> GetGameInfoAsync(int gameId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/games/info")
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<int> { gameId }), Encoding.UTF8, "application/json")
            };

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(line) && line.StartsWith("data:"))
                {
                    // Remove the "data:" prefix and trim the line
                    var messageJson = line.Substring("data:".Length).Trim();

                    var message = JsonSerializer.Deserialize<SSEMessage>(messageJson);
                    if (message.Event == "game")
                    {
                        var game = JsonSerializer.Deserialize<BEGame>(message.Data);
                        return game;
                    }
                    else if (message.Event == "error")
                    {
                        throw new Exception(message.Data);
                    }
                    // Handle other message types if needed
                }
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using GamesServersMonitor.Domain.Contracts.Services;
using GamesServersMonitor.Domain.Entities;
using GamesServersMonitor.Infrastructure.Messaging.MediatR;
using MediatR;

namespace GamesServersMonitor.App.Services
{
    public class EmulatorService : IEmulatorService, IRequestHandler<ServerUpdateRequest>
    {
        private readonly ILogger<EmulatorService> _logger;
        private readonly IUnitOfWork _Uow;
        private readonly HttpClient _httpClient;
        private readonly IMediator _mediator;

        private CancellationTokenSource? _cancellationTokenSource;

        private ServerUpdateRequest updateRequest { get; set; } = new() { Stop = true };

        public EmulatorService(ILogger<EmulatorService> logger, IUnitOfWork uow, HttpClient httpClient, IMediator mediator)
        {
            _logger = logger;
            _Uow = uow;
            _httpClient = httpClient;
            _mediator = mediator;
        }

        /// <summary>
        /// Starts long polling on the server to receive updates and add them to the database.
        /// </summary>
        /// <param name="numOfServers">The number of game servers to initialize in the emulator.</param>
        /// <param name="gameIds">The list of game IDs to use for the game servers.</param>
        /// <param name="_interval">The polling interval, in milliseconds.</param>
        public async Task StartAsync(int numOfServers, List<int> gameIds, int _interval, Func<bool, IActionResult> callback)
        {
            #region init
            // Initialize the game servers
            var gameServers = await InitEmulatorAsync(numOfServers, gameIds);
            if(gameServers != null &&  gameServers.Count == numOfServers)
            {
                callback(true);
            }
            else
            {
                throw new InvalidOperationException("No game servers found! Please start the emulator first.");
            }
            #endregion

            // Create a cancellation token source
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Create the request URL with the interval
                var requestUrl = $"start_emulator?interval={_interval}";

                // Send the HTTP GET request to the server with the cancellation token
                using var response = await _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead, _cancellationTokenSource.Token).ConfigureAwait(false);

                // Check if the response was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a stream
                    using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                    // Loop through the Server-Sent Events stream
                    await ProcessServerSentEventsStreamAsync(stream).ConfigureAwait(false);
                }
                else
                {
                    // Throw an exception if the response was not successful
                    throw new Exception($"Failed to get an update server: {response.ReasonPhrase}");
                }
            }
            catch (OperationCanceledException ex)
            {
                // Handle the cancellation by logging a message
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _logger.LogInformation($"Polling canceled: {ex.Message}");
                }
            }
        }

        private async Task ProcessServerSentEventsStreamAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                // Read the next line from the stream
                var line = await reader.ReadLineAsync().ConfigureAwait(false);

                // Check if the line is not empty and starts with "data:"
                if (!string.IsNullOrEmpty(line) && line.StartsWith("data:"))
                {
                    await ProcessServerUpdateAsync(line).ConfigureAwait(false);
                }
            }
        }

        private async Task ProcessServerUpdateAsync(string line)
        {
            // Remove the "data:" prefix and trim the line
            var serverUpdateJson = line.Substring("data:".Length).Trim();

            // Deserialize the server update from JSON
            var serverUpdate = JsonConvert.DeserializeObject<ServerUpdate>(serverUpdateJson);

            //check if the server upddate is not null
            if (serverUpdate != null)
            {
                // Send the server update to the mediator
                if (!updateRequest.Stop && updateRequest.ServerId == serverUpdate.ServerId)
                {
                    var serverUpdateResponse = new ServerUpdateResponse { ServerUpdate = serverUpdate };
                    await _mediator.Send(serverUpdateResponse).ConfigureAwait(false);
                }

                // Add the server update to the database and save changes
                await _Uow.ServerUpdateRepository.AddAsync(serverUpdate).ConfigureAwait(false);
                await _Uow.SaveAsync().ConfigureAwait(false);

                // Log a message with the server update
                _logger.LogInformation("Server update received for server id: {@serverUpdate.ServerId}", serverUpdate.ServerId);
            }
        }

        public async Task<string> StopAsync()
        {
            _cancellationTokenSource?.Cancel();
            var response = await _httpClient.PostAsync("stop_emulator", null).ConfigureAwait(false);
            //_httpClient?.Dispose();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// init the emulatur and get a list of game servers
        /// </summary>
        /// <param name="n"> the number of game servers to generate</param>
        /// <param name="gameIds"> a list of gameIds the emulator can choose from to generate a game server</param>
        /// <returns>the list of generated game servers</returns>
        /// <exception cref="Exception"></exception>
        private async Task<List<GameServer>> InitEmulatorAsync(int numOfServers, List<int> gameIds)
        {
            var request = new
            {
                n = numOfServers,
                game_ids = gameIds
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("init_emulator", content).ConfigureAwait(false);


            if (response.IsSuccessStatusCode)
            {
                var gameServersJson = await response.Content.ReadAsStringAsync();
                var gameServers = JsonConvert.DeserializeObject<IEnumerable<GameServer>>(gameServersJson);

                await _Uow.GameServerRepository.DeleteAllAsync().ConfigureAwait(false);
                await _Uow.ServerUpdateRepository.DeleteAllAsync().ConfigureAwait(false);

                await _Uow.GameServerRepository.AddRangeAsync(gameServers).ConfigureAwait(false);
                await _Uow.SaveAsync().ConfigureAwait(false);

                _logger.LogInformation("Emulator initialized successfully with {numOfServers} servers and {gameIdsCount} game IDs.", numOfServers, gameIds.Count);

                return gameServers.ToList();
            }
            else
            {
                var message = $"Failed to initialize the emulator: {response.ReasonPhrase}";

                _logger.LogError(message);

                throw new Exception(message);
            }
        }

        public Task Handle(ServerUpdateRequest request, CancellationToken cancellationToken)
        {
            updateRequest = request;
            return Task.CompletedTask;
        }
    }
}

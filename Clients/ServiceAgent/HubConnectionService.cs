﻿using BL.BE;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ServiceAgent.Contracts;

namespace ServiceAgent
{
    public class HubConnectionService : IHubConnectionService
    {
        private readonly HubConnection _connection;
        private readonly ILogger<HubConnectionService> _logger;

        //public event EventHandler<List<BEGame>> GameReceived;

        public HubConnectionService(ILogger<HubConnectionService>? logger = null)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = logger ?? loggerFactory.CreateLogger<HubConnectionService>();

            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5770/serverHub")
                .WithAutomaticReconnect()
                .Build();
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Starting SignalR connection...");
            }

            try
            {
                await _connection.StartAsync(cancellationToken).ConfigureAwait(false);
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("SignalR connection started successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start SignalR connection.");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Stopping SignalR connection...");
            }

            try
            {
                await _connection.StopAsync(cancellationToken).ConfigureAwait(false);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("SignalR connection stopped successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to stop SignalR connection.");
                throw;
            }
        }

        public async Task SendMessageAsync<T>(string methodName, T message)
        {
            await _connection.SendAsync(methodName, message).ConfigureAwait(false);
        }

        public void On<T>(string methodName, Action<T> handler)
        {
            _connection.On(methodName, handler);
        }

        public async IAsyncEnumerable<T> GetAllAsync<T>(string methodName, object? arg1 = null,object? arg2 = null)
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                await _connection.StartAsync();
            }
            if (arg1 == null) //get GameServers
            await foreach (var obj in _connection.StreamAsync<string>(methodName))
            {
                var deserializedObject = JsonSerializer.Deserialize<T>(obj);
                if (deserializedObject != null)
                    yield return deserializedObject;
            }
            else            //get ServerUpdates
            await foreach (var obj in _connection.StreamAsync<string>(methodName, arg1, arg2))
            {
                var deserializedObject = JsonSerializer.Deserialize<T>(obj);
                if (deserializedObject != null)
                    yield return deserializedObject;
            }   
        }

        public async IAsyncEnumerable<T> GetAllLiveAsync<T>(string methodName, Action<T> handler, object? arg1 = null, object? arg2 = null)
        {
            if (arg1 != null)
            {
                // Remove all existing handlers before registering the new one
                _connection.Remove("LiveServerUpdate");

                // Register the new call
                _connection.On<string>("LiveServerUpdate", obj =>
                {
                    var deserializedObject = JsonSerializer.Deserialize<T>(obj);
                    if (deserializedObject != null)
                        handler(deserializedObject);
                });
            }
            await foreach(var obj in GetAllAsync<T>(methodName, arg1, arg2)) { yield return obj;}
        }

        public void StopLiveUpdate()
        {
            _connection.InvokeAsync("StopServerUpdates");
        }

        public void SetTimeout(int? timeout)
        {
            _connection.ServerTimeout = TimeSpan.FromSeconds(timeout ?? 60);
        }

        public async void Dispose()
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Disposing SignalR connection...");
            }

            try
            {
                await StopAsync();
                await _connection.DisposeAsync();

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("SignalR connection disposed successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to dispose SignalR connection.");
                throw;
            }
        }
    }

}

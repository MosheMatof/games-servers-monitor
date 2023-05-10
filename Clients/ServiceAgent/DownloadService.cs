using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ServiceAgent.Contracts;

namespace ServiceAgent
{
    public class DownloadService : IDownloadService
    {
        private readonly ILogger<DownloadService> _logger;
        private HttpClient? httpClient;

        public DownloadService(ILogger<DownloadService>? logger = null)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            _logger = logger ?? loggerFactory.CreateLogger<DownloadService>();
        }

        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            try
            {
                // Ensure that the HttpClient has been initialized
                if (httpClient == null)
                {
                    throw new InvalidOperationException("HttpClient has not been started.");
                }

                // Combine the relative URI with the BaseAddress to form the absolute URI
                var absoluteUri = new Uri(httpClient.BaseAddress, imageUrl);

                _logger.LogInformation($"Downloading image from URL: {absoluteUri}");

                // Download the image data as a byte array
                var imageData = await httpClient.GetByteArrayAsync(absoluteUri);

                _logger.LogInformation($"Image downloaded successfully.");

                return imageData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading image from URL: {imageUrl}");

                // Handle any errors that occur during image download
                throw new FaultException(ex.Message);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (httpClient == null)
            {
                _logger.LogInformation("Initializing HttpClient.");

                httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://images.igdb.com/");

                _logger.LogInformation("HttpClient initialized.");
            }

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if (httpClient != null)
            {
                _logger.LogInformation($"Disposing HttpClient.");

                httpClient.Dispose();
                await Task.Yield(); // Yield control to allow resources to be released

                _logger.LogInformation($"HttpClient disposed.");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (httpClient != null)
                {
                    _logger.LogInformation($"Disposing HttpClient.");

                    httpClient.Dispose();
                    httpClient = null;

                    _logger.LogInformation($"HttpClient disposed.");
                }
            }
        }

    }
}

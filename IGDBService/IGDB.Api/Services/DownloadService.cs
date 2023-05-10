using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using IGDB.Domain.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace IGDB.Api.Services
{
    public class DownloadService : IDownloadService
    {
        private readonly ILogger<DownloadService> _logger;
        private HttpClient? _httpClient;

        public DownloadService(ILogger<DownloadService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            try
            {
                // Ensure that the HttpClient has been initialized
                if (_httpClient == null)
                {
                    throw new InvalidOperationException("HttpClient has not been started.");
                }

                // Combine the relative URI with the BaseAddress to form the absolute URI
                var absoluteUri = new Uri(_httpClient.BaseAddress, imageUrl);

                _logger.LogInformation($"Downloading image from URL: {absoluteUri}");

                // Download the image data as a byte array
                var imageData = await _httpClient.GetByteArrayAsync(absoluteUri);

                _logger.LogInformation($"Image downloaded successfully.");

                return imageData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading image from URL: {imageUrl}");

                // Handle any errors that occur during image download
                throw new Exception(ex.Message);
            }
        }
    }
}

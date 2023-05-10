using IGDB;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text;
using IGDB.Domain.Contracts;
using System.Text.Json;
using IGDB.Domain.Entities;

namespace IGDB.Api.Services
{
    public class GameService : IGameService
    {
        private readonly IGDBClient _client;
        private readonly ILogger<GameService> _logger;
        private readonly IDownloadService _downloadService;

        public GameService(ILogger<GameService> logger, IDownloadService downloadService,IGDBClient client)
        {
            this._logger = logger;
            _downloadService = downloadService;
            _client = client;
        }

        public async Task<List<int>> GetGamesIdsAsync(int amount)
        {
            _logger.LogInformation("Retrieving list of top {amount} game IDs.", amount);

            var query = $@"
            fields id;
            where total_rating_count > 0;
            sort total_rating_count desc;
            limit {amount};
        ";

            try
            {
                var games = await _client.QueryAsync<IGDB.Models.Game>(IGDBClient.Endpoints.Games, query);
                var gamesIds = games.Select(game => (int)game.Id).ToList();
                _logger.LogInformation("Retrieved list of top {amount} game IDs.", amount);
                return gamesIds;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("No internet connection!");
                throw ex;
            }
        }

        public async Task<List<T>> GetGamesAsync<T>(int amount, string sortField = "total_rating_count", string fields = "id")
        {
            _logger.LogInformation("Retrieving list of top {amount} game IDs.", amount);

            var query = $@"
                fields {fields};
                where total_rating_count > 0;
                sort {sortField} desc;
                limit {amount};
            ";

            try
            {
                var games = await _client.QueryAsync<T>(IGDBClient.Endpoints.Games, query);
                var gamesData = games.ToList();
                _logger.LogInformation("Retrieved list of top {amount} games.", amount);
                return gamesData;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("No internet connection!");
                throw ex;
            }
        }

        public async Task<string> GetGameInfoByIdAsync(int gameId)
        {
            //var query = @"
            //    fields id, name, summary, total_rating_count, cover.url, url;
            //    where id = {gameId};
            //";
            var query = $@"
                fields id, name, summary, total_rating_count, cover.url, url;
                where id = {gameId};
                ";
            //query = query.Replace("id", "CAST(id as int)");

            try
            {
                var games = await _client.QueryAsync<IGDB.Models.Game>(IGDBClient.Endpoints.Games, query);
                var game = games.First();
                if (game != null)
                {
                    var imageBytes = await _downloadService.DownloadImageAsync(game.Cover.Value.Url);
                    var imageBase64 = Convert.ToBase64String(imageBytes);

                    var data = new Game()
                    {
                        IGDBId = (int)game.Id,
                        Name = game.Name,
                        Description = game.Summary,
                        ImageUrl = game.Cover.Value.Url,
                        ImageBase64 = imageBase64,
                        Link = game.Url
                    };

                    return JsonSerializer.Serialize<Game>(data);
                }
                else
                {
                    return null;
                }
            }
            catch (RestEase.ApiException ex)
            {
                // Log the error
                _logger.LogError($"Error calling IGDB API: {ex.Message}\n{ex.Content}");

                // Rethrow the exception to propagate it up the call stack
                //throw;
                return null;
            }
        }
    }
}

using IGDB.Domain.Contracts;
using IGDB.Infrastructure.Massaging.Events;
using IGDB.Infrastructure.Massaging.Services;
using RestEase.Implementation;

namespace IGDB.Api.Services
{
    public class GetNewGamesEventHandler : BackgroundService
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IGameService _gameService;
        private readonly ILogger<GetNewGamesEventHandler> _logger;

        public GetNewGamesEventHandler(IRabbitMQService rabbitMQService,IGameService gameService, ILogger<GetNewGamesEventHandler> logger)
        {
            _rabbitMQService = rabbitMQService;
            _gameService = gameService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            stoppingToken.ThrowIfCancellationRequested();

            try
            {
                _rabbitMQService.GetNewGamesRequest(async request =>
                {
                    _logger.LogInformation($"Received request to get {request.Amount} new games.");

                    var gamesIds = await _gameService.GetGamesIdsAsync(request.Amount);
                    _rabbitMQService.GetNewGamesResponse(request, gamesIds);

                    _logger.LogInformation($"Sent response with {gamesIds.Count} new game(s).");
                }
            );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error accur while processing the request: {ex.Message}");
            }
        }
    }
}

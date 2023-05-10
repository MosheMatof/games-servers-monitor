using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IGDB;
using IGDB.Api.Services;
using IGDB.Domain.Contracts;
using IGDB.Infrastructure.SSE;
using System.Text.Json;
using RestEase;

namespace IGDB.Api.Controllers
{
    [Route("api/games")]
    [ApiController] 
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("info")]
        public async Task<IActionResult> GetGameInfo([FromBody] List<int> gameIds)
        {
            if (gameIds == null || gameIds.Count == 0)
            {
                return BadRequest("Invalid jsonGame IDs.");
            }

            try
            {
                var response = Response;
                response.Headers.Add("Content-Type", "text/event-stream");
                response.Headers.Add("Cache-Control", "no-cache");

                // Set the status code before starting the SSE stream
                response.StatusCode = 200;

                await response.StartAsync();

                foreach (var gameId in gameIds)
                {
                    var jsonGame = await _gameService.GetGameInfoByIdAsync(gameId);

                    if (jsonGame != null)
                    {
                        var message = new SSEMessage
                        {
                            Event = "game",
                            Data = jsonGame
                        };
                        var jsonMessage = JsonSerializer.Serialize(message);
                        await response.WriteAsync($"data:{jsonMessage}\n\n");
                        await response.Body.FlushAsync();
                    }
                    else
                    {
                        // Send an error SSE message instead of returning a 404 status code
                        var message = new SSEMessage
                        {
                            Event = "error",
                            Data = $"Game with ID {gameId} not found."
                        };
                        var jsonMessage = JsonSerializer.Serialize(message);
                        await response.WriteAsync($"data:{jsonMessage}\n\n");
                        await response.Body.FlushAsync();
                    }
                }

                // End the SSE stream
                await response.WriteAsync(":sse-end\n\n");
                await response.Body.FlushAsync();

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                // Check if the response has been started before sending an SSEMessage
                if (Response.HasStarted)
                {
                    var message = new SSEMessage
                    {
                        Event = "error",
                        Data = "An error occurred while processing your request."
                    };
                    var jsonMessage = JsonSerializer.Serialize(message);
                    await Response.WriteAsync($"data:{jsonMessage}\n\n");
                    await Response.Body.FlushAsync();

                    return new EmptyResult();
                }
                else
                {
                    // If the response has not been started, return a 500 status code
                    return StatusCode(500);
                }

                // Log the error here or handle it as appropriate for your application
            }
        }
    }
}

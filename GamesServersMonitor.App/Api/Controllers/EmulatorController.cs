using GamesServersMonitor.App.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace GamesServersMonitor.App.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmulatorController : ControllerBase
    {
        private readonly IEmulatorService _emulatorService;
        private readonly IGetGamesService _getGamesService;

        public EmulatorController(IEmulatorService emulatorService, IGetGamesService getGamesService)
        {
            _emulatorService = emulatorService;
            _getGamesService = getGamesService;
        }

        [HttpPost]
        public async Task<IActionResult> Init([FromBody] object request)
        {
            dynamic data = JsonConvert.DeserializeObject<dynamic>(request.ToString());

            int numOfGames, numOfServers = 0, interval = 0;
            if (int.TryParse(data.NumOfGames.ToString(), out numOfGames) &&
                int.TryParse(data.NumOfServers.ToString(), out numOfServers) &&
                int.TryParse(data.Interval.ToString(), out interval))
            {
                try
                {
                    var gameIds = await _getGamesService.GetNewGamesAsync(numOfGames);
                    await _emulatorService.StartAsync(numOfServers, gameIds, interval, succsess =>
                    {
                        if (succsess)
                            return Ok();
                        else
                            return BadRequest();
                    }).ConfigureAwait(false);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Invalid request data");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Stop()
        {
            var response = await _emulatorService.StopAsync().ConfigureAwait(false);
            return Ok(response);
        }
    }
}

using GamesServersMonitor.Infrastructure.Messaging.RabbitMQ;
using GamesServersMonitor.Infrastructure.Messaging.RabbitMQ.Events;
using System.Threading;

namespace GamesServersMonitor.App.Services
{
    public class GetGamesService : IGetGamesService
    {
        private readonly IRabbitMQService _rabbitMQService;

        public GetGamesService(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        public async Task<List<int>> GetNewGamesAsync(int amount, int timeout = 10000)
        {
            var request = new NewGamesRequest { Amount = amount, CorrelationId = Guid.NewGuid().ToString() };

            var responseTaskCompletionSource = new TaskCompletionSource<NewGamesResponse>();

            await _rabbitMQService.GetNewGamesRequest(request, response =>
            {
                // Set the result on the TaskCompletionSource when the response is received
                responseTaskCompletionSource.SetResult(response);
                return Task.CompletedTask;
            }).ConfigureAwait(false);

            // Wait for either the TaskCompletionSource to complete or the timeout to elapse
            var completedTask = await Task.WhenAny(responseTaskCompletionSource.Task, Task.Delay(timeout)).ConfigureAwait(false);

            if (completedTask == responseTaskCompletionSource.Task)
            {
                // The TaskCompletionSource completed, so return the response
                var response = await responseTaskCompletionSource.Task;
                return response.GamesIds;
            }
            else
            {
                // The timeout elapsed before the TaskCompletionSource completed, so throw an exception
                throw new TimeoutException($"Timed out waiting for response after {timeout * 0.001} seconds.");
            }
        }
    }
}

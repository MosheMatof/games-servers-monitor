using GamesServersMonitor.Domain.Contracts.Services;
using GamesServersMonitor.Domain.Entities;
using GamesServersMonitor.Infrastructure.Messaging.MediatR;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace GamesServersMonitor.Infrastructure.Hubs
{
    public class ServerHub : Hub, IRequestHandler<ServerUpdateResponse>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IMediator _mediator;

        private static IClientProxy? _clientProxy;
        public ServerHub(IMediator mediator,IServiceScopeFactory serviceScopeFactory) 
        {
            _serviceScopeFactory = serviceScopeFactory;
            _mediator = mediator;
        }

        public async IAsyncEnumerable<string> GetServers()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var _Uow = scope.ServiceProvider.GetService<IUnitOfWork>();

            await foreach (var server in await _Uow.GameServerRepository.GetAllAsync())
            {
                // Use System.Text.Json to serialize each GameServer entity
                var json = System.Text.Json.JsonSerializer.Serialize(server);
                yield return json;
            }
        }

        public async IAsyncEnumerable<string> GetServerUpdates(int id, DateTime? dateTime)
        {
            GetServerUpdatesFromMediator(id);
            _clientProxy = Clients.Caller;

            using var scope = _serviceScopeFactory.CreateScope();
            var _Uow = scope.ServiceProvider.GetService<IUnitOfWork>();

            dateTime ??= DateTime.MinValue;
            var ServerUpdates = await _Uow.ServerUpdateRepository.
                GetAllAsync(filter: su => su.ServerId == id && su.TimeStamp > dateTime,
                            orderBy: updateServers => updateServers.OrderBy(su => su.TimeStamp));

            await foreach (var server in ServerUpdates)
            {
                // Use System.Text.Json to serialize each GameServer entity
                var json = System.Text.Json.JsonSerializer.Serialize(server);
                yield return json;
            }
        }

        private void GetServerUpdatesFromMediator(int id)
        {
            _mediator.Send(new ServerUpdateRequest {Stop = false, ServerId = id });
        }

        public void StopServerUpdates()
        {
            _mediator.Send(new ServerUpdateRequest() { Stop = true });
        }

        public Task Handle(ServerUpdateResponse request, CancellationToken cancellationToken)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(request.ServerUpdate);

            _clientProxy?.SendAsync("LiveServerUpdate", json, cancellationToken: cancellationToken);
            return Task.CompletedTask;
        }
    }
}

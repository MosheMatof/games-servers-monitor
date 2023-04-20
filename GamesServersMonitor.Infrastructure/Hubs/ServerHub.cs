using GamesServersMonitor.Domain.Contracts.Services;
using GamesServersMonitor.Domain.Entities;
using GamesServersMonitor.Infrastructure.Messaging.MediatR;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace GamesServersMonitor.Infrastructure.Hubs
{
    public class ServerHub : Hub, IRequestHandler<ServerUpdateResponse>
    {
        private readonly IUnitOfWork _Uow;
        private readonly IMediator _mediator;

        private IClientProxy? _clientProxy;
        public ServerHub(IUnitOfWork uow,IMediator mediator) 
        {
            _Uow = uow;
            _mediator = mediator;
        }

        public async IAsyncEnumerable<string> GetServers()
        {
            await foreach (var server in await _Uow.GameServerRepository.GetAllAsync())
            {
                // Use System.Text.Json to serialize each GameServer entity
                var json = System.Text.Json.JsonSerializer.Serialize(server);
                yield return json;
            }
        }

        public async IAsyncEnumerable<string> GetServerUpdates(int id)
        {
            GetServerUpdatesFromMediator(id);
            _clientProxy = Clients.Caller;

            var ServerUpdates = await _Uow.ServerUpdateRepository.
                GetAllAsync(filter: su => su.ServerId == id,
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

            _clientProxy?.SendAsync("BEServerUpdate", json, cancellationToken: cancellationToken);
            return Task.CompletedTask;
        }
    }
}

using GamesServersMonitor.Infrastructure.Messaging.MediatR;
using MediatR;

namespace GamesServersMonitor.App.Services
{
    public class EmulatorRequestHandler : IRequestHandler<ServerUpdateRequest>
    {
        private readonly IEmulatorService _emulatorService;

        public EmulatorRequestHandler(IEmulatorService emulatorService)
        {
            _emulatorService = emulatorService;
        }

        public Task Handle(ServerUpdateRequest request, CancellationToken cancellationToken)
        {
            return _emulatorService.Handle(request, cancellationToken);
        }
    }

}

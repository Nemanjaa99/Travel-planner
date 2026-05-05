using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace TravelService
{
    internal sealed class TravelServiceFabric : StatelessService
    {
        private readonly WebApplication _app;

        public TravelServiceFabric(StatelessServiceContext context, WebApplication app)
            : base(context)
        {
            _app = app;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
            => new ServiceInstanceListener[0];

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await _app.StartAsync(cancellationToken);
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
    }
}
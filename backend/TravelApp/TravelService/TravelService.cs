using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace TravelService
{
    internal sealed class TravelService : StatelessService
    {
        public TravelService(StatelessServiceContext context)
            : base(context) { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
            => new ServiceInstanceListener[0];
    }
}
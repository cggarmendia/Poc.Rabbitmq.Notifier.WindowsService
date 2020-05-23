using System;
using Autofac;
using Vueling.Extensions.Library.DI;
using Vueling.Messaging.RabbitMqEndpoint.Impl.ServiceLibrary.Consumers.Dispatching;

namespace Poc.Rabbitmq.Notifier.WindowsService.Helpers
{
    /// <summary>
    /// This class allows the Endpoint to resolve application services used by event/command handlers
    /// </summary>
    [RegisterService]
    public class EndpointResolver : IDependencyResolver
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CustomRules.Maintenability", "VY1000:GlobalNotUseServiceLocatorPattern")]
        public static IContainer Container { get; set; }

        public dynamic Resolve(Type type)
        {
            return Container.Resolve(type);
        }
    }
}

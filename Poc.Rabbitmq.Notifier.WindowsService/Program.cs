using System.Diagnostics;
using Poc.Rabbitmq.Notifier.WindowsService.Bootstrapping;
using Poc.Rabbitmq.Notifier.WindowsService.CommandHandlers;
using Poc.Rabbitmq.Notifier.WindowsService.Helpers;
using Vueling.Configuration.Library;

namespace Poc.Rabbitmq.Notifier.WindowsService
{
    static class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
        static void Main(string[] args)
        {
            VuelingEnvironment.InitializeCurrentForApplication("Vueling.Contingency.CrmNotifier.WindowsService");
            EndpointConfiguration.SetGetRabbitMqConnections();
            var service = MessageConsumerBuilder.RegisterMessageHandlers(                                                
                        typeof(CrmNotifierRequestHandler))
                    .RegisterCustomisations()
                    .Build();
            Trace.TraceInformation("Service created");
#if (!DEBUG)
            System.ServiceProcess.ServiceBase.Run(service);
#else
            service.Start();
#endif
        }
    }
}

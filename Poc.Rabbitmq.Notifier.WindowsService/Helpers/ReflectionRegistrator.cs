using System.Diagnostics;
using Autofac;
using Microsoft.Identity.Client;
using Poc.Rabbitmq.Notifier.WindowsService.CommandHandlers;
using Poc.Rabbitmq.Notifier.WindowsService.Configuration;
using Vueling.DIRegister.Custom.ServiceLibrary;

namespace Poc.Rabbitmq.Notifier.WindowsService.Helpers
{
    public class ReflectionRegistrator : DICustom
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CustomRules.Maintenability", "VY1000:GlobalNotUseServiceLocatorPattern")]
        public IContainer Container { get; set; }

        protected override void CustomDependenciesRegister(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<Configuration.Configuration>().As<IConfiguration>().SingleInstance();
            containerBuilder.RegisterType<CrmNotifierRequestHandler>().As<Vueling.Messaging.RabbitMqEndpoint.Contracts.ServiceLibrary.Consumers.Events.IHandleEvent<Vueling.Contingency.Message.Domain.CrmNotifier.CrmNotifierRequest>>();
            containerBuilder.RegisterType<Application.Configuration.Configuration>().As<Application.Configuration.IConfiguration>().SingleInstance();
            containerBuilder.RegisterType<Infrastructure.Configuration.Configuration>().As<Infrastructure.Configuration.IConfiguration>().SingleInstance();
            
            containerBuilder.Register(x =>
            {
                var config = x.Resolve<Infrastructure.Configuration.IConfiguration>();
                return ConfidentialClientApplicationBuilder.Create(config.AzureClientId)
                    .WithTenantId(config.AzureTenant)
                    .WithClientSecret(config.AzureSecret)
                    .Build();
            }).As<IConfidentialClientApplication>().SingleInstance();
        
            Trace.TraceInformation("Execute override of CustomDependenciesRegister.");
        }

        protected override void ResolveAfterBuildContainer(IContainer container)
        {
            Container = container;
        }
    }
}
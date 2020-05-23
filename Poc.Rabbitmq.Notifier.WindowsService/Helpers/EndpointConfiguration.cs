using System.Collections.Generic;
using Vueling.Configuration.Library;
using Vueling.Messaging.RabbitMqEndpoint.Contracts.ServiceLibrary.Configuration;

namespace Poc.Rabbitmq.Notifier.WindowsService.Helpers
{
    public class EndpointConfiguration : Vueling.Messaging.RabbitMqEndpoint.Contracts.ServiceLibrary.Configuration.IEndpointConfiguration
    {
        private static List<RabbitMqConnection> _rabbitMqConnections;
        public static void SetGetRabbitMqConnections()
        {
            var rabbitMqConnection = VuelingEnvironment.Current.GetRabbitMqConnection("Internal");
            _rabbitMqConnections = new List<RabbitMqConnection>()
            {
                new RabbitMqConnection()
                {
                    HostName= rabbitMqConnection.HostName,
                    ManagementPort= rabbitMqConnection.ManagementPort,
                    Password= rabbitMqConnection.Password,
                    Port=rabbitMqConnection.Port,
                    User= rabbitMqConnection.User,
                    VirtualHost=rabbitMqConnection.VirtualHost
                }
            };
        }
        public List<RabbitMqConnection> GetRabbitMqConnections()
        {
            return _rabbitMqConnections;
        }
        public string GetApplicationName()
        {
            return "Vueling.Contingency.CrmNotifier.WindowsService";
        }
    }
}
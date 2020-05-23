namespace Poc.Rabbitmq.Notifier.Infrastructure.Configuration
{
    public interface IConfiguration
    {
        string AzureClientId { get; }
        string AzureSecret { get; }
        string AzureTenant { get; }
        string AzureScope { get; }
    }
}

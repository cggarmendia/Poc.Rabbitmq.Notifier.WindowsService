namespace Poc.Rabbitmq.Notifier.WindowsService.Configuration
{
    public interface IConfiguration
    {
        short PerConsumerConcurrency { get; }
    }
}

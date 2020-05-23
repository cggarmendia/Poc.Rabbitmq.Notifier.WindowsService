using Autofac;

namespace Poc.Rabbitmq.Notifier.WindowsService.Bootstrapping.Fluent
{
    public interface IBuildWindowsService
    {
        ConsumerWindowsService Build();
        IContainer BuildDebug();
    }
}

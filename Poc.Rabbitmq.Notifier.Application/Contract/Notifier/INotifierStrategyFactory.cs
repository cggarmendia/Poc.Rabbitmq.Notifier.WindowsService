namespace Poc.Rabbitmq.Notifier.Application.Contract.Notifier
{
    public interface INotifierStrategyFactory
    {
        T GetNotifierStrategy<T>(params object[] objects)
            where T : class, INotifierStrategy;
    }
}
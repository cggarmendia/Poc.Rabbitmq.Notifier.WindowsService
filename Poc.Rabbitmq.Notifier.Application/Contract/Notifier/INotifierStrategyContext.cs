using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier;

namespace Poc.Rabbitmq.Notifier.Application.Contract.Notifier
{
    public interface INotifierStrategyContext
    {
        void SetStrategy(string processType);
        void Notify(CrmNotifierDto parameter);
    }
}
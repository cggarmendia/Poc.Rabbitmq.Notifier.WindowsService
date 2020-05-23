using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier;

namespace Poc.Rabbitmq.Notifier.Application.Contract.Notifier
{
    public interface INotifierStrategy
    {
        void Notify(CrmNotifierDto parameter);
    }
}

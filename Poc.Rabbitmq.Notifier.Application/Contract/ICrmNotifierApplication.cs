using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier;

namespace Poc.Rabbitmq.Notifier.Application.Contract
{
    public interface ICrmNotifierApplication
    {
        void Notify(CrmNotifierDto parameter);
    }
}
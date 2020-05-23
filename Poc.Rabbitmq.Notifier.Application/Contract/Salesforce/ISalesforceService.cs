using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier.Salesforce.Request;

namespace Poc.Rabbitmq.Notifier.Application.Contract.Salesforce
{
    public interface ISalesforceService
    {
        T SendRequestPost<T>(SalesforceRequestDto salesforceParam, string salesforceUrl) where T : new();

        T SendRequestPut<T>(SalesforceRequestDto salesforceParam, string salesforceUrl) where T : new();
    }
}

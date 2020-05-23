using System.Collections.Generic;
using System.Net;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier;

namespace Poc.Rabbitmq.Notifier.Infrastructure.Contract.RequestHandler
{
    public interface IRequestHandlerService
    {
        KeyValuePair<HttpStatusCode, string> Post(PostRequestDto parameters);
        KeyValuePair<HttpStatusCode, string> Put(PostRequestDto parameters);
    }
}

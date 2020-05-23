using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier.Salesforce.Request;
using Poc.Rabbitmq.Notifier.Application.Contract.Salesforce;
using Poc.Rabbitmq.Notifier.Infrastructure.Contract.Azure;
using Poc.Rabbitmq.Notifier.Infrastructure.Contract.RequestHandler;

namespace Poc.Rabbitmq.Notifier.Application.Implementation.Salesforce
{
    public class SalesforceService : ISalesforceService
    {
        #region Properties
        private readonly IAuthenticationAzureService _authenticationAzureService;
        private readonly IRequestHandlerService _requestHandlerService;
        #endregion

        #region Ctros.
        public SalesforceService(IAuthenticationAzureService authenticationAzureService,
            IRequestHandlerService requestHandlerService)
        {
            _authenticationAzureService = authenticationAzureService;
            _requestHandlerService = requestHandlerService;
        }
        #endregion

        #region Public_Methods
        public T SendRequestPost<T>(SalesforceRequestDto salesforceParam, string salesforceUrl) where T : new()
        {
            var postParam = GetPostParam(salesforceParam, salesforceUrl);
            var postResponse = _requestHandlerService.Post(postParam);

            return JsonConvert.DeserializeObject<T>(postResponse.Value);
        }

        public T SendRequestPut<T>(SalesforceRequestDto salesforceParam, string salesforceUrl) where T : new()
        {
            var postParam = GetPostParam(salesforceParam, salesforceUrl);
            var putResponse = _requestHandlerService.Put(postParam);

            return JsonConvert.DeserializeObject<T>(putResponse.Value);
        }
        #endregion

        #region Private_Methods
        private PostRequestDto GetPostParam(SalesforceRequestDto salesforceParam, string salesforcelUrl)
        {
            var postParam = new PostRequestDto()
            {
                Url = salesforcelUrl,
                RequestData = GetRequestData(salesforceParam),
                TokenValue = GetTokenValue()
            };

            return postParam;
        }

        private string GetRequestData(SalesforceRequestDto salesforceParam)
        {
            if (typeof(SalesforceBulkInsertListRequestDto) == salesforceParam.GetType())
            {
                return JsonConvert.SerializeObject(
                    ((SalesforceBulkInsertListRequestDto)salesforceParam).RequestInsertParams);
            }

            if (typeof(SalesforceNotifyListRequestDto) == salesforceParam.GetType())
            {
                return JsonConvert.SerializeObject(
                    ((SalesforceNotifyListRequestDto)salesforceParam).SalesforceNotifyList);
            }

            var requestData = JsonConvert.SerializeObject(salesforceParam);

            if (typeof(SalesforceRefundEmailRequestDto) == salesforceParam.GetType())
            {

                if (((SalesforceRefundEmailRequestDto)salesforceParam).Attributes.ContainsKey("ExtraProces"))
                {
                    var strings = requestData.Split(new string[] { "\"ExtraProces\"" }, StringSplitOptions.None);
                    requestData = strings[0] + ((SalesforceRefundEmailRequestDto)salesforceParam).Attributes["ExtraProces"].Substring(1, ((SalesforceRefundEmailRequestDto)salesforceParam).Attributes["ExtraProces"].Length - 1) + "}";
                }
            }

            return requestData;
        }

        private KeyValuePair<string, string> GetTokenValue()
        {
            var authBearerToken = _authenticationAzureService.GetToken();
            return new KeyValuePair<string, string>("Bearer", authBearerToken);
        }
        #endregion
    }
}

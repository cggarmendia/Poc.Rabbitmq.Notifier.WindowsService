using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier;
using Poc.Rabbitmq.Core.Domain.Extension;
using Poc.Rabbitmq.Notifier.Infrastructure.Contract.RequestHandler;
using Poc.Rabbitmq.Notifier.Infrastructure.Exceptions;

namespace Poc.Rabbitmq.Notifier.Infrastructure.Implementation.RequestHandler
{
    public class RequestHandlerService : IRequestHandlerService
    {
        #region Properties
        private const string MediaType = "application/json";
        #endregion

        #region Ctor
        public RequestHandlerService()
        {

        }
        #endregion

        #region Public_Methods
        public KeyValuePair<HttpStatusCode, string> Post(PostRequestDto parameters)
        {
            try
            {
                KeyValuePair<HttpStatusCode, string> responseBody;

                using (var client = new HttpClient())
                {
                    HttpClientConfig(client, parameters.TokenValue);
                    using (var postAsync = client.PostAsync(parameters.Url, GetStringContent(parameters.RequestData)))
                    {
                        postAsync.Wait();
                        responseBody = GetResponseContentByStatusCodeAsync(postAsync.Result);
                    }
                }

                return responseBody;
            }
            catch (Exception ex)
            {
                var errorMessage = $@"Error in: {GetType().FullName}, method : Post, postParam: {parameters.RequestData.Replace(new [] { '}', '{', '\\', '\"' }, string.Empty)}, innerExceptionType: {ex.GetType()}, innerExceptionMessage: {ex.Message}";
                throw new RequestHandlerPostException(errorMessage);
            }
        }

        public KeyValuePair<HttpStatusCode, string> Put(PostRequestDto parameters)
        {
            try
            {
                KeyValuePair<HttpStatusCode, string> responseBody;

                using (var client = new HttpClient())
                {
                    HttpClientConfig(client, parameters.TokenValue);
                    using (var putAsync = client.PutAsync(parameters.Url, GetStringContent(parameters.RequestData)))
                    {
                        putAsync.Wait();
                        responseBody = GetResponseContentByStatusCodeAsync(putAsync.Result);
                    }
                }

                return responseBody;
            }
            catch (Exception ex)
            {
                var errorMessage = $@"Error in: {GetType().FullName}, method : put, put: {parameters.RequestData.Replace(new[] { '}', '{', '\\', '\"' }, string.Empty)}, innerExceptionType: {ex.GetType()}, innerExceptionMessage: {ex.Message}";
                throw new RequestHandlerPostException(errorMessage);
            }
        }
        #endregion

        #region Private_Methods
        private static StringContent GetStringContent(string requestData)
        {
            return new StringContent(requestData, Encoding.UTF8, MediaType);
        }

        private void HttpClientConfig(HttpClient client, KeyValuePair<string, string> tokenValue)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
            client.DefaultRequestHeaders.Add("User-Agent", "Poc.Rabbitmq.Notifier");
            client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenValue.Key, tokenValue.Value);
        }

        private KeyValuePair<HttpStatusCode, string> GetResponseContentByStatusCodeAsync(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var errorMessage = $@"Error in: {GetType().FullName}, method : GetResponseContentByStatusCodeAsync, status code : {response.StatusCode}.";
                throw new Exception(errorMessage);
            }

            return new KeyValuePair<HttpStatusCode, string>(response.StatusCode, response.Content.ReadAsStringAsync().Result);
        }
        #endregion
    }
}

using System;
using System.Diagnostics;
using Microsoft.Identity.Client;
using Poc.Rabbitmq.Notifier.Infrastructure.Configuration;
using Poc.Rabbitmq.Notifier.Infrastructure.Contract.Azure;
using Poc.Rabbitmq.Notifier.Infrastructure.Exceptions;

namespace Poc.Rabbitmq.Notifier.Infrastructure.Implementation.Azure
{
    public class AuthenticationAzureService : IAuthenticationAzureService
    {
        #region Properties
        private readonly IConfiguration _config;
        private readonly IConfidentialClientApplication _confidentialClientApplication;
        #endregion

        #region Ctor
        public AuthenticationAzureService(IConfiguration config,
            IConfidentialClientApplication confidentialClientApplication)
        {
            _config = config;
            _confidentialClientApplication = confidentialClientApplication;
        }
        #endregion

        #region Public_Methods
        public string GetToken()
        {
            try
            {
                var tokenForClient = _confidentialClientApplication.AcquireTokenForClient(new[] {_config.AzureScope}).ExecuteAsync();

                tokenForClient.Wait();

                return tokenForClient.Result.AccessToken;
            }
            catch (Exception ex)
            {
                var errorMessage = $@"Error in: {GetType().FullName}, message : {ex.Message}, exception: {ex} , method: getToken";
                Trace.TraceError(errorMessage);
                throw new AuthenticationAzureException(errorMessage);
            }
        }
        #endregion

        #region Private methods
        #endregion
    }
}

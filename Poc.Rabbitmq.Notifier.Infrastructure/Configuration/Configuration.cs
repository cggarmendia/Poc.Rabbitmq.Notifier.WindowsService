using System;
using System.Configuration;
using System.Diagnostics;
using Poc.Rabbitmq.Core.Domain.Exception;

namespace Poc.Rabbitmq.Notifier.Infrastructure.Configuration
{
    public class Configuration : IConfiguration
    {
        #region Properties
        public string AzureClientId { get; internal set; }

        public string AzureSecret { get; internal set; }

        public string AzureTenant { get; internal set; }

        public string AzureScope { get; internal set; }
        #endregion

        #region Ctor.
        public Configuration()
        {
            try
            {
                LoadCustomSettings();
            }
            catch (ConfigurationInitializationException) { throw; }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Error initializing configuration in {0}: {1}. {2}",
                    this.GetType().FullName, ex.Message, ex);

                Trace.TraceError(errorMessage);
                throw new ConfigurationInitializationException(errorMessage);
            }
        }
        #endregion

        #region
        private void LoadCustomSettings()
        {
            AzureClientId = FindKey("AzureClientId");
            AzureSecret = FindKey("AzureSecret");
            AzureTenant = FindKey("AzureTenant");
            AzureScope = FindKey("AzureScope");
        }

        private string FindKey(string keyVar)
        {
            return ConfigurationManager.AppSettings[keyVar];
        }
        #endregion
    }
}

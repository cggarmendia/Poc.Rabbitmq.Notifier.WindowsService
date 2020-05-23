using System.Configuration;
using System.Diagnostics;
using Poc.Rabbitmq.Core.Domain.Exception;

namespace Poc.Rabbitmq.Notifier.Application.Configuration
{
    public class Configuration : IConfiguration
    {
        #region Properties.
        public string SalesforceSendEmailUrl { get; internal set; }

        public string SalesforceTemplateCode { get; internal set; }

        public string SalesforceNotifyAgencyUrl { get; internal set; }

        public string SalesforceVoluntaryRefundTemplateCode { get; internal set; }
        #endregion

        #region Ctor.
        public Configuration()
        {
            try
            {
                LoadCustomSettings();
            }
            catch (ConfigurationInitializationException) { throw; }
            catch (System.Exception ex)
            {
                var errorMessage = string.Format("Error initializing configuration in {0}: {1}. {2}",
                    this.GetType().FullName, ex.Message, ex);

                Trace.TraceError(errorMessage);
                throw new ConfigurationInitializationException(errorMessage);
            }
        }
        #endregion

        #region Private_Methods
        private void LoadCustomSettings()
        {
            SalesforceSendEmailUrl = FindKey("SalesforceSendEmailUrl");
            SalesforceTemplateCode = FindKey("SalesforceTemplateCode");
            SalesforceVoluntaryRefundTemplateCode = FindKey("SalesforceVoluntaryRefundTemplateCode");
            SalesforceNotifyAgencyUrl = FindKey("SalesforceNotifyAgencyUrl");
        }

        private string FindKey(string keyVar)
        {
            return ConfigurationManager.AppSettings[keyVar];
        }
        #endregion
    }
}

namespace Poc.Rabbitmq.Notifier.Application.Configuration
{
    public interface IConfiguration
    {
        string SalesforceSendEmailUrl { get; }
        string SalesforceNotifyAgencyUrl { get; }
        string SalesforceTemplateCode { get; }
        string SalesforceVoluntaryRefundTemplateCode { get; }
    }
}

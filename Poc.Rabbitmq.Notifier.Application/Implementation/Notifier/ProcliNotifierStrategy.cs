using System.Collections.Generic;
using System.Globalization;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier.Salesforce.Request;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier.Salesforce.Response;
using Poc.Rabbitmq.Core.Domain.Exception.CrmNotifier;
using Poc.Rabbitmq.Notifier.Application.Configuration;
using Poc.Rabbitmq.Notifier.Application.Contract.Notifier;
using Poc.Rabbitmq.Notifier.Application.Contract.Salesforce;

namespace Poc.Rabbitmq.Notifier.Application.Implementation.Notifier
{
    public class ProcliNotifierStrategy : INotifierStrategy
    {
        #region Properties
        private readonly IConfiguration _configuration;
        private readonly ISalesforceService _salesforceService;
        #endregion

        #region Ctor.
        public ProcliNotifierStrategy(IConfiguration configuration, ISalesforceService salesforceService)
        {
            _configuration = configuration;
            _salesforceService = salesforceService;
        }
        #endregion

        #region Public_Methods
        public void Notify(CrmNotifierDto parameter)
        {
            try
            {
                var salesforceResponse = _salesforceService.SendRequestPost<SalesforceResponseDto>(GetSalesforceParam(parameter), _configuration.SalesforceSendEmailUrl);
                if (!salesforceResponse.Result)
                {
                    throw new ProcliNotifierException($"Salesforce Internal Error. ErrorCode: {salesforceResponse.ErrorCode}. ErrorMessage: {salesforceResponse.ErrorMessage}.");
                }
            }
            catch (System.Exception ex)
            {
                var errorMessage =
                    $@"Error in: {GetType().Name}, method : SendEmail, RecordLocator: {parameter.RecordLocator}, innerExceptionType: {ex.GetType()}, innerExceptionMessage: {ex.Message}";
                throw new ProcliNotifierException(errorMessage);
            }
        }
        #endregion

        #region Private_Methods
        private SalesforceRefundEmailRequestDto GetSalesforceParam(CrmNotifierDto parameter)
        {
            return new SalesforceRefundEmailRequestDto()
            {
                CarrierCode = parameter.CarrierCode,
                ContactEmail = parameter.Email,
                ContactFirstname = parameter.ContactFirstname,
                ContactLastname = parameter.ContactLastname,
                ContactLocale = parameter.CultureCode,
                TemplateCode = _configuration.SalesforceTemplateCode,
                Attributes = new Dictionary<string, string>()
                        {
                            {"BookingId" , parameter.BookingId},
                            {"BookingPnr" , parameter.RecordLocator},
                            {"RefundType" , "CREDIT_SHELL"},
                            {"RefundCode" , parameter.RecordLocator},
                            {"RefundAmount" , parameter.Amount.ToString("0.##", CultureInfo.InvariantCulture)},
                            {"RefundExpirationDate" , parameter.ExpirationDate.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)},
                            {"InventoryLegId" , parameter.InventoryLegId.ToString()},
                            {"FlightCarrier" , parameter.CarrierCode},
                            {"FlightDateLT" , parameter.FlightDateLT.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)},
                            {"FlightDepartureIATA" , parameter.FlightDepartureIATA},
                            {"FlightArrivalIATA" , parameter.FlightArrivalIATA},
                            {"RefundCurrencyCode" , parameter.Currency},
                            {"FlightNumber" , parameter.FlightNumber},
                            {"RefundIncrementType", parameter.RefundType },
                            {"RefundValue" , parameter.RefundValue.ToString("0.##", CultureInfo.InvariantCulture) },
                            {"RefundCurrencyCode" , parameter.RefundCurrencyCode },
                            {"RefundOriginalAmount" , parameter.RefundOriginalAmount.ToString("0.##",  CultureInfo.InvariantCulture) }
                        }
            };
        }
        #endregion
    }
}

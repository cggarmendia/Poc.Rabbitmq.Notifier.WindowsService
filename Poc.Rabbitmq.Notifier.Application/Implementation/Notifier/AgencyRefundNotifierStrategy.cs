using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier.Salesforce.Request;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier.Salesforce.Response;
using Poc.Rabbitmq.Core.Domain.Exception.CrmNotifier;
using Poc.Rabbitmq.Notifier.Application.Configuration;
using Poc.Rabbitmq.Notifier.Application.Contract.Notifier;
using Poc.Rabbitmq.Notifier.Application.Contract.Salesforce;

namespace Poc.Rabbitmq.Notifier.Application.Implementation.Notifier
{
    public class AgencyRefundNotifierStrategy : INotifierStrategy
    {
        #region Properties
        private readonly IConfiguration _configuration;
        private readonly ISalesforceService _salesforceService;
        #endregion

        #region Ctor.
        public AgencyRefundNotifierStrategy(IConfiguration configuration, ISalesforceService salesforceService)
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
                var salesforceResponse = _salesforceService.SendRequestPut<List<SalesforceNotifyResponseDto>>(GetSalesforceParam(parameter), _configuration.SalesforceNotifyAgencyUrl);
                if (!string.IsNullOrEmpty(salesforceResponse.FirstOrDefault()?.Error))
                {
                    throw new AgencyRefundNotifierException($"Salesforce Internal Error. Error: {salesforceResponse.FirstOrDefault()?.Error}.");
                }
            }
            catch (System.Exception ex)
            {
                var errorMessage =
                    $@"Error in: {GetType().Name}, method : Notify, booking: {parameter.RecordLocator}, innerExceptionType: {ex.GetType()}, innerExceptionMessage: {ex.Message}";
                throw new AgencyRefundNotifierException(errorMessage);
            }
        }
        #endregion

        #region Private_Methods
        private SalesforceNotifyListRequestDto GetSalesforceParam(CrmNotifierDto parameter)
        {
            return parameter.Result
                ? GetSalesforceNotifyListRequestDtoWithoutError(parameter)
                : GetSalesforceNotifyListRequestDtoWithError(parameter);
        }

        private static SalesforceNotifyListRequestDto GetSalesforceNotifyListRequestDtoWithError(CrmNotifierDto parameter)
        {
            return new SalesforceNotifyListRequestDto()
            {
                SalesforceNotifyList = new List<SalesforceNotifyRequestDto>()
                {
                    new SalesforceNotifyRequestDto()
                    {
                        ExternalId = parameter.ExternalId,
                        Result = parameter.Result,
                        ErrorCode = parameter.ErrorCode,
                        ErrorMessage = parameter.ErrorMessage,
                        RefundType = "AGENCY_REFUND"
                    }
                }
            };
        }

        private static SalesforceNotifyListRequestDto GetSalesforceNotifyListRequestDtoWithoutError(CrmNotifierDto parameter)
        {
            return new SalesforceNotifyListRequestDto()
            {
                SalesforceNotifyList = new List<SalesforceNotifyRequestDto>()
                {
                    new SalesforceNotifyRequestDto()
                    {
                        ExternalId = parameter.ExternalId,
                        Result = parameter.Result,
                        RefundType = "AGENCY_REFUND",
                        RefundCode = parameter.RecordLocator,
                        RefundAmount = parameter.Amount.ToString("0.##", CultureInfo.InvariantCulture),
                        RefundOriginalAmount = parameter.RefundOriginalAmount.ToString("0.##", CultureInfo.InvariantCulture),
                        RefundCurrencyCode = parameter.Currency,
                        RefundIncrementType = parameter.RefundType,
                        RefundIncrementValue = parameter.RefundValue,
                        RefundIncrementCurrencyCode = parameter.RefundCurrencyCode,
                        RefundExpirationDate = parameter.ExpirationDate,
                        Flights = GetFlights(parameter)
                    }
                }
            };
        }

        private static List<FlightDto> GetFlights(CrmNotifierDto parameter)
        {
            var flightProcessedList = new List<FlightDto>();
            parameter.FlightProcessedList.ForEach(flightProcessed =>
            {
                flightProcessedList.Add(new FlightDto()
                {
                    InventoryLegId = flightProcessed.InventoryLegId,
                    Carrier = flightProcessed.Carrier,
                    FlightNumber = flightProcessed.FlightNumber,
                    FlightDateLT = flightProcessed.FlightDateLT,
                    DepartureIATA = flightProcessed.DepartureIATA,
                    ArrivalIATA = flightProcessed.ArrivalIATA
                });
            });
            return flightProcessedList;
        }
        #endregion
    }
}

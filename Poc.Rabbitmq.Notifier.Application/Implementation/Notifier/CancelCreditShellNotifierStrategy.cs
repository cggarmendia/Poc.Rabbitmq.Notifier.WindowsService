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
    public class CancelCreditShellNotifierStrategy : INotifierStrategy
    {
        #region Properties
        private readonly IConfiguration _configuration;
        private readonly ISalesforceService _salesforceService;
        #endregion

        #region Ctor.
        public CancelCreditShellNotifierStrategy(IConfiguration configuration, ISalesforceService salesforceService)
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
                var salesforceResponse = _salesforceService.SendRequestPut<List<SalesforceNotifyResponseDto>>(GetSalesforceNotifyListRequestDto(parameter), _configuration.SalesforceNotifyAgencyUrl);
                if (!string.IsNullOrEmpty(salesforceResponse.FirstOrDefault()?.Error))
                {
                    throw new CancelCreditShellNotifierException($"Salesforce Internal Error. Error: {salesforceResponse.FirstOrDefault()?.Error}.");
                }
            }
            catch (System.Exception ex)
            {
                var errorMessage =
                    $@"Error in: {GetType().Name}, method : Notify, booking: {parameter.RecordLocator}, innerExceptionType: {ex.GetType()}, innerExceptionMessage: {ex.Message}";
                throw new CancelCreditShellNotifierException(errorMessage);
            }
        }
        #endregion

        #region Private_Methods
        private static SalesforceNotifyListRequestDto GetSalesforceNotifyListRequestDto(CrmNotifierDto parameter)
        {
            return new SalesforceNotifyListRequestDto()
            {
                SalesforceNotifyList = new List<SalesforceNotifyRequestDto>()
                {
                    new SalesforceNotifyRequestDto()
                    {
                        ExternalId = parameter.ExternalId,
                        Result = parameter.Result,
                        RefundType = "CANCEL_CREDIT_SHELL",
                        RefundAmount = parameter.Amount.ToString("0.##", CultureInfo.InvariantCulture),
                        RefundOriginalAmount = parameter.RefundOriginalAmount.ToString("0.##", CultureInfo.InvariantCulture),
                        RefundCurrencyCode = parameter.RefundCurrencyCode,
                        ErrorCode = parameter.ErrorCode,
                        ErrorMessage = parameter.ErrorMessage
                    }
                }
            };
        }
        #endregion
    }
}

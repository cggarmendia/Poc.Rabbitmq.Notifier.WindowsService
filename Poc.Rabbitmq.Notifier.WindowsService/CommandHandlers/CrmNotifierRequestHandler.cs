using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier;
using Poc.Rabbitmq.Core.Message.Domain.Common;
using Poc.Rabbitmq.Core.Message.Domain.CrmNotifier;
using Poc.Rabbitmq.Notifier.Application.Contract;
using Poc.Rabbitmq.Notifier.WindowsService.Helpers;
using Vueling.Messaging.RabbitMqEndpoint.Contracts.ServiceLibrary.Consumers.Events;
using Vueling.Messaging.RabbitMqEndpoint.Contracts.ServiceLibrary.Policies;

namespace Poc.Rabbitmq.Notifier.WindowsService.CommandHandlers
{
    public class CrmNotifierRequestHandler : IHandleEvent<CrmNotifierRequest>
    {
        private readonly ICrmNotifierApplication _businessLogic;

        public CrmNotifierRequestHandler(ICrmNotifierApplication businessLogic)
        {
            _businessLogic = businessLogic;
        }

        public void Handle(IEventProxy proxy)
        {
            var command = proxy.GetEvent<CrmNotifierRequest>();

            try
            {
                Handle(command);
                proxy.Completed();
            }
            catch (Exception ex)
            {
                var errorMessage =
                    $@"Principal Error in: {GetType().Name}, method : Handle, booking: {command.RecordLocator}, innerExceptionType: {ex.GetType()}, innerExceptionMessage: {ex.Message}";
                Trace.TraceError(errorMessage, ex);
                proxy.Failed(PolicyHint.IsTransient, errorMessage);
            }
        }

        public void Handle(CrmNotifierRequest command)
        {
            LoggingHelper.TraceInformation(() => $"CrmNotifierHandler.Handle Start RecordLocator: {command.RecordLocator}");

            var flightProcessedList = GetFlightProcessedList(command.FlightProcessedList);
            var isAnyFlightProcessed = flightProcessedList.Any();

            _businessLogic.Notify(new CrmNotifierDto()
            {
                ErrorCode = command.ErrorCode,
                ErrorMessage = command.ErrorMessage,
                ExternalId = command.ExternalId,
                Currency = command.Currency,
                Amount = command.Amount,
                RefundOriginalAmount = command.OriginalAmount,
                ExpirationDate = command.ExpirationDate,
                BookingId = command.BookingId,
                ContactFirstname = command.ContactFirstname,
                ContactLastname = command.ContactLastname,
                CultureCode = command.CultureCode,
                Email = command.Email,
                CarrierCode = isAnyFlightProcessed ? flightProcessedList.First().Carrier : string.Empty,
                FlightArrivalIATA = isAnyFlightProcessed ? flightProcessedList.First().ArrivalIATA : string.Empty,
                FlightDateLT = isAnyFlightProcessed ? flightProcessedList.First().FlightDateLT : DateTime.MinValue,
                FlightDepartureIATA = isAnyFlightProcessed ? flightProcessedList.First().DepartureIATA : string.Empty,
                FlightNumber = isAnyFlightProcessed ? flightProcessedList.First().FlightNumber : string.Empty,
                InventoryLegId = isAnyFlightProcessed ? long.Parse(flightProcessedList.First().InventoryLegId) : long.MinValue,
                RecordLocator = command.RecordLocator,
                Result = command.Result,
                RefundType = command.RefundType,
                RefundValue = command.RefundValue,
                RefundCurrencyCode = command.RefundCurrencyCode,
                FlightProcessedList = flightProcessedList,
                ProcessType = command.ProcessType
            });

            LoggingHelper.TraceInformation(() => $"CrmNotifierHandler.Handle End RecordLocator: {command.RecordLocator}");
        }

        private static List<FlightProcessed> GetFlightProcessedList(List<FlightProcessedRequest> flightProcessedRequestsList)
        {
            return flightProcessedRequestsList != null && flightProcessedRequestsList.Any()
                ?
                    flightProcessedRequestsList.Select(flightProcessed => new FlightProcessed()
                    {
                        Carrier = flightProcessed.CarrierCode,
                        InventoryLegId = flightProcessed.InventoryLegId,
                        DepartureIATA = flightProcessed.FlightDepartureIATA,
                        ArrivalIATA = flightProcessed.FlightArrivalIATA,
                        FlightDateLT = flightProcessed.FlightDateLT,
                        FlightNumber = flightProcessed.FlightNumber,
                        Processed = true
                    }).ToList()
                :
                    new List<FlightProcessed>();
        }
    }
}

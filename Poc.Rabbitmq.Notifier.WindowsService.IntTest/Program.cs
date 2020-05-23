using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autofac;
using Poc.Rabbitmq.Notifier.WindowsService.Bootstrapping;
using Poc.Rabbitmq.Notifier.WindowsService.CommandHandlers;
using Poc.Rabbitmq.Notifier.WindowsService.Helpers;
using Vueling.Configuration.Library;
using Vueling.Messaging.RabbitMqEndpoint.Contracts.ServiceLibrary.Consumers.Events;

namespace Poc.Rabbitmq.Notifier.WindowsService.IntTest
{
    static class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
        static void Main(string[] args)
        {
            VuelingEnvironment.InitializeCurrentForApplication("Vueling.Contingency.CrmNotifier.WindowsService");
            EndpointConfiguration.SetGetRabbitMqConnections();
            var buildDebug = MessageConsumerBuilder.RegisterMessageHandlers(
                    typeof(CrmNotifierRequestHandler))
                .RegisterCustomisations()
                .BuildDebug();
            Trace.TraceInformation("Service created");
            try
            {
                var handleEvent = EndpointResolver.Container.Resolve<IHandleEvent<CrmNotifierRequest>>() as CrmNotifierRequestHandler;
                handleEvent.Handle(new CrmNotifierRequest()
                {
                    Result = false,
                    ExternalId = "Test",
                    ExpirationDate = DateTime.Now,
                    Currency = "EUR",
                    ErrorMessage = "no anencgdd",
                    Amount = 100.00M,
                    ErrorCode = "fsfdff",
                    RecordLocator = "VY4443",
                    BookingId = "13241241",
                    ContactFirstname = "Test",
                    ContactLastname = "Test",
                    CultureCode = "es-ES",
                    Email = "test@test.com",
                    ProcessType = "CancelCreditShell",
                    RefundCurrencyCode = "EUR",
                    RefundType = "Percentage",
                    RefundValue = 0.00M,
                    OriginalAmount = 100.00M,
                    FlightProcessedList = new List<FlightProcessedRequest>()
                    {
                        new FlightProcessedRequest()
                        {
                            CarrierCode = "Test",
                            InventoryLegId = "1234567",
                            FlightDepartureIATA = "Test",
                            FlightArrivalIATA = "Test",
                            FlightDateLT = DateTime.Now,
                            FlightNumber = "Test",
                        },
                        new FlightProcessedRequest()
                        {
                            CarrierCode = "Test2",
                            InventoryLegId = "1234567",
                            FlightDepartureIATA = "Test2",
                            FlightArrivalIATA = "Test2",
                            FlightDateLT = DateTime.Now,
                            FlightNumber = "Test2",
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                try
                {
                    var errorMessage =
                        $@"Principal Error in: Test, method : Handle, booking: Test, innerExceptionType: {ex.GetType()}, innerExceptionMessage: {ex.Message}";

                    Trace.TraceError(errorMessage, ex);
                }
                catch (Exception exInner)
                {
                    
                    Console.WriteLine(exInner.Message);
                }
            }
        }
    }
}

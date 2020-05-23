using System;
using System.Diagnostics;
using System.ServiceProcess;
using Vueling.Messaging.RabbitMqEndpoint.Contracts.ServiceLibrary.Policies;
using Vueling.Messaging.RabbitMqEndpoint.Impl.ServiceLibrary.Endpoints;
using Vueling.Messaging.RabbitMqEndpoint.Impl.ServiceLibrary.Policies;
using Vueling.Messaging.RabbitMqEndpoint.Impl.ServiceLibrary.Policies.Detectors.BuiltIn;

namespace Poc.Rabbitmq.Notifier.WindowsService.IntTest
{
    public partial class ConsumerWindowsService : ServiceBase
    {
        private readonly IEndpointConsumerManager _endpointManager;

        public ConsumerWindowsService(IEndpointConsumerManager endpointManager)
        {
            _endpointManager = endpointManager;

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                FailPolicy.CreateFor(typeof(CrmNotifierRequest))
                    .ClassifyAsTransient(new SqlTransientErrorDetector())
                    .OnTransientError(retryLimit: 3,
                        retryPeriod: TimeSpan.FromMinutes(1),
                        periodType: PeriodType.ExponentialBackoff,
                        exponent: 2,
                        retryLimitAction: FailAction.SendToFailExchange)
                    .OnPersistentError(FailAction.SendToFailExchange);

                Endpoint.InitializeAsConsumer(_endpointManager)
                    .HandleEvent<CrmNotifierRequest>()                    
                    .WithSingleConsumer(perConsumerConcurrency: 1)
                    .Start();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not start windows service: " + ex);
            }
        }

        protected override void OnStop()
        {
            try
            {
                Endpoint.Stop(TimeSpan.FromSeconds(60));
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not stop windows service: " + ex);
            }
        }

  
        public void Start()
        {
            OnStart(null);

            Console.WriteLine("Started. Press any key to shutdown the consumer(s)");
            Console.Read();
            Console.WriteLine("Shutting down, this may take a few seconds...");
            Endpoint.Stop(TimeSpan.FromSeconds(60));
        }
    }
}

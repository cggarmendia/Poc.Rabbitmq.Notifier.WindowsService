using Poc.Rabbitmq.Core.Domain.Constant;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier;
using Poc.Rabbitmq.Notifier.Application.Configuration;
using Poc.Rabbitmq.Notifier.Application.Contract.Notifier;
using Poc.Rabbitmq.Notifier.Application.Contract.Salesforce;

namespace Poc.Rabbitmq.Notifier.Application.Implementation.Notifier.Strategy
{
    public class NotifierStrategyContext : INotifierStrategyContext
    {
        #region Properties
        private readonly IConfiguration _configuration;
        private readonly ISalesforceService _salesforceService;
        private readonly INotifierStrategyFactory _notifierStrategyFactory;
        private INotifierStrategy NotifierStrategy { get; set; }
        #endregion

        #region Ctor.
        public NotifierStrategyContext(IConfiguration configuration, 
            ISalesforceService salesforceService,
            INotifierStrategyFactory notifierStrategyFactory)
        {
            _configuration = configuration;
            _salesforceService = salesforceService;
            _notifierStrategyFactory = notifierStrategyFactory;
        }
        #endregion

        #region Public_Methods
        public void SetStrategy(string processType)
        {
            if (processType.Equals(ProcessTypeConst.CancelCreditShell))
            {
                NotifierStrategy = _notifierStrategyFactory.GetNotifierStrategy<CancelCreditShellNotifierStrategy>(_configuration, _salesforceService);
            }
            else if (processType.Equals(ProcessTypeConst.AgencyCancelAndAddPayment))
            {
                NotifierStrategy = _notifierStrategyFactory.GetNotifierStrategy<AgencyRefundNotifierStrategy>(_configuration, _salesforceService);
            }
            else if (processType.Equals(ProcessTypeConst.CreateVoluntary))
            {
                NotifierStrategy = _notifierStrategyFactory.GetNotifierStrategy<VoluntaryRefundNotifierStrategy>(_configuration, _salesforceService);
            }
            else if (processType.Equals(ProcessTypeConst.PROCLI))
            {
                NotifierStrategy = _notifierStrategyFactory.GetNotifierStrategy<ProcliNotifierStrategy>(_configuration, _salesforceService);
            }
        }

        public void Notify(CrmNotifierDto parameter)
        {
            NotifierStrategy.Notify(parameter);
        }
        #endregion
    }
}

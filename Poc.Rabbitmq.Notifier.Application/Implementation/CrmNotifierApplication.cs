using System;
using Poc.Rabbitmq.Core.Domain.Dto.CrmNotifier;
using Poc.Rabbitmq.Core.Domain.Exception.CrmNotifier;
using Poc.Rabbitmq.Notifier.Application.Contract;
using Poc.Rabbitmq.Notifier.Application.Contract.Notifier;

namespace Poc.Rabbitmq.Notifier.Application.Implementation
{
    public class CrmNotifierApplication : ICrmNotifierApplication
    {
        #region Properties
        private readonly INotifierStrategyContext _notifierStrategyContext;
        #endregion

        #region Ctors.
        public CrmNotifierApplication(INotifierStrategyContext notifierStrategyContext)
        {
            _notifierStrategyContext = notifierStrategyContext;
        }
        #endregion

        #region Public_Methods
        public void Notify(CrmNotifierDto parameter)
        {
            try
            {
                _notifierStrategyContext.SetStrategy(parameter.ProcessType);
                _notifierStrategyContext.Notify(parameter);
            }
            catch (Exception ex)
            {
                var errorMessage =
                    $@"Error in: {GetType().Name}, method : Notify, RecordLocator: {parameter.RecordLocator}, innerExceptionType: {ex.GetType()}, innerExceptionMessage: {ex.Message}";
                throw new CrmNotifierException(errorMessage);
            }
        }
        #endregion
    }
}

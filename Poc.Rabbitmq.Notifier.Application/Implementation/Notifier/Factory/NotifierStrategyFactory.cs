using System.Linq;
using System.Threading;
using Poc.Rabbitmq.Core.Infrastructure.Cache.Contracts;
using Poc.Rabbitmq.Notifier.Application.Contract.Notifier;

namespace Poc.Rabbitmq.Notifier.Application.Implementation.Notifier.Factory
{
    public class NotifierStrategyFactory : INotifierStrategyFactory
    {
        #region Properties
        private readonly ICacheComponent _cacheComponent;
        private readonly Mutex _mutex;
        #endregion

        #region Ctor.
        public NotifierStrategyFactory(ICacheComponent cacheComponent)
        {
            _cacheComponent = cacheComponent;
            _mutex = new Mutex();
        }
        #endregion

        #region Public_Methods
        public T GetNotifierStrategy<T>(params object[] objects)
            where T : class, INotifierStrategy
        {
            var validationName = typeof(T).FullName;

            var validationInstance = default(T);

            if (_cacheComponent.ContainsKey(validationName))
            {
                validationInstance = _cacheComponent.TryGetValue<T>(validationName);
            }
            else
            {
                try
                {
                    if (_mutex.WaitOne())
                    {
                        var constructorInfo = typeof(T).GetConstructor(objects.Select(objectInstance => objectInstance.GetType()).ToArray());
                        if (constructorInfo != null)
                        {
                            validationInstance = (T)constructorInfo.Invoke(objects);

                            if (!_cacheComponent.ContainsKey(validationName))
                                _cacheComponent.TryAdd(validationName, validationInstance);
                        }
                    }
                }
                finally
                {
                    _mutex.ReleaseMutex();
                }
            }

            return validationInstance;
        }
        #endregion
    }
}

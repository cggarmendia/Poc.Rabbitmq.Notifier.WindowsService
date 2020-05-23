using System;
using System.Runtime.Serialization;

namespace Poc.Rabbitmq.Notifier.Infrastructure.Exceptions
{
    [Serializable]
    public class RequestHandlerPostException : Exception
    {
        public RequestHandlerPostException()
        {

        }
        public RequestHandlerPostException(string message) : base(message)
        {

        }
        public RequestHandlerPostException(string message, Exception innerException) : base(message, innerException)
        {

        }
        protected RequestHandlerPostException(SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {

        }
    }
}

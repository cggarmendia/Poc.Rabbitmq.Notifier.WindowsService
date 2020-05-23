using System;
using System.Runtime.Serialization;

namespace Poc.Rabbitmq.Notifier.Infrastructure.Exceptions
{
    [Serializable]
    public class AuthenticationAzureException : Exception
    {
        public AuthenticationAzureException()
        {

        }
        public AuthenticationAzureException(string message) : base(message)
        {

        }
        public AuthenticationAzureException(string message, Exception innerException) : base(message, innerException)
        {

        }
        protected AuthenticationAzureException(SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {

        }
    }
}

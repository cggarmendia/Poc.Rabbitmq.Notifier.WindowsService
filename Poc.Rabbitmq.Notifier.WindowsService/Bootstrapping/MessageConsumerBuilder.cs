using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Autofac;
using Poc.Rabbitmq.Notifier.WindowsService.Bootstrapping.Fluent;
using Poc.Rabbitmq.Notifier.WindowsService.Helpers;
using Vueling.DIRegister.AssemblyDiscovery.ServiceLibrary;
using Vueling.DIRegister.AssemblyDiscovery.ServiceLibrary.DTO;
using Vueling.Messaging.RabbitMqEndpoint.Impl.ServiceLibrary.Consumers.Dispatching;
using Vueling.Messaging.RabbitMqEndpoint.Impl.ServiceLibrary.Endpoints;

namespace Poc.Rabbitmq.Notifier.WindowsService.Bootstrapping
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CustomRules.Maintenability", "VY1001:GlobalUseDecoratedServices")]
    public class MessageConsumerBuilder : IRegisterCustomisations, IBuildWindowsService
    {
        #region .: Boilerplate (don't change) :.

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CustomRules.Maintenability", "VY1000:GlobalNotUseServiceLocatorPattern")]
        private readonly ContainerBuilder _builder;
        private readonly RegisterDefinition _registerDefinition;
        private readonly ReflectionRegistrator _reflectionRegistrator;
        

        private MessageConsumerBuilder(params Type[] eventHandlers)
        {
            _registerDefinition = new RegisterDefinition()
            {
                IgnoreTypes = new List<Type>()
                {
                    typeof(IDependencyResolver),                    
                },
                ExecutingAssembly = Assembly.GetExecutingAssembly(),
                AdditionalEntryServices = new List<Type>() {
                    typeof(ConsumerWindowsService),
                    typeof(IEndpointConsumerManager)
                },
                DefaultServiceLifetimeScope = LifetimeScopes.InstancePerDependency
            };

            foreach (var eventHandler in eventHandlers)
                _registerDefinition.AdditionalEntryServices.Add(eventHandler);

            _builder = new ContainerBuilder();
            _reflectionRegistrator = new ReflectionRegistrator();
        }
                
        public static IRegisterCustomisations RegisterMessageHandlers(params Type[] eventHandlers)
        {
            return new MessageConsumerBuilder(eventHandlers);
        }

        public ConsumerWindowsService BuildWithVerbose()
        {
            return Build(true);
        }

        public ConsumerWindowsService Build()
        {
            return Build(false);
        }

        public IContainer BuildDebug()
        {
            try
            {
                RegisterDependencies(true);
                UpdateBuilder(_reflectionRegistrator.Container);
                EndpointResolver.Container = _reflectionRegistrator.Container;
                return _reflectionRegistrator.Container;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not build windows service: " + ex);
                throw;
            }
        }

        private ConsumerWindowsService Build(bool verbose)
        {
            try
            {
                RegisterDependencies(verbose);
                UpdateBuilder(_reflectionRegistrator.Container);
                EndpointResolver.Container = _reflectionRegistrator.Container;
                return _reflectionRegistrator.Container.Resolve<ConsumerWindowsService>();
            }
            catch(Exception ex)
            {
                Trace.TraceError("Could not build windows service: " + ex);
                throw;
            }
        }

        private void RegisterDependencies(bool verbose)
        {
            if (verbose)
                _reflectionRegistrator.EnableVerboseTrace();

            _reflectionRegistrator.RegisterDependencies(_registerDefinition);
        }

        private void UpdateBuilder(IContainer container)
        {
            _builder.Update(container);
        }

        #endregion .: Boilerplate (don't change) :.

        #region .: Add Your Custom DI Code here :.

        public IBuildWindowsService RegisterCustomisations()
        {
            _builder.RegisterType<EndpointResolver>().As<IDependencyResolver>();

            // custom registrations


            // customise RegisterDefinition


            return this;
        }

        #endregion
    }
}

﻿using System;
using System.Diagnostics;
using Poc.Rabbitmq.Core.Domain.Exception;
using Vueling.Configuration.Library;
using Vueling.Extensions.Library.DI;

namespace Poc.Rabbitmq.Notifier.WindowsService.Configuration
{
    [RegisterConfiguration]
    public class Configuration : IConfiguration
    {
        private readonly VuelingEnvironment _currentConfig;

        public Configuration()
        {
            try
            {
                _currentConfig = LoadCurrentConfig();
                LoadCustomSettings();
            }
            catch (ConfigurationInitializationException) { throw; }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Error initializing configuration in {0}: {1}. {2}",
                    this.GetType().FullName, ex.Message, ex);

                Trace.TraceError(errorMessage);
                throw new ConfigurationInitializationException(errorMessage);
            }
        }

        private VuelingEnvironment LoadCurrentConfig()
        {
            if (!VuelingEnvironment.IsInitialized)
            {
                var errorMessage = string.Format("VuelingEnvironment not initialized in {0}.",
                    this.GetType().FullName);

                Trace.TraceError(errorMessage);
                throw new ConfigurationInitializationException(errorMessage);
            }

            VuelingEnvironment.InitializeLibrary("Vueling.Contingency.CrmNotifier.WindowsService");

            return VuelingEnvironment.Current;
        }

        private void LoadCustomSettings()
        {
            PerConsumerConcurrency = Convert.ToInt16(FindKey("PerConsumerConcurrency"));
        }

        public short PerConsumerConcurrency { get; internal set; }


        private string FindKey(string keyVar)
        {
            return _currentConfig.GetCustomSetting(keyVar);
        }
    }
}

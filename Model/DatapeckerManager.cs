using Datapecker.Agent.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    public static class DatapeckerManager
    {
        //поля
        private static object _initializeLock = new object();
        private static ServiceManager _serviceManager;
        private static ConfigManager _configManger;
        private static ConfigModel _configuration;


        //свойства
        public static ConfigModel Configuration
        {
            get
            {
                Initialize();
                return _configuration;
            }
            set
            {
                _configuration = value;
            }
        }
        
        

        //методы
        private static void Initialize(ConfigModel configuration = null)
        {
            lock (_initializeLock)
            {
                if (_serviceManager == null)
                {
                    _serviceManager = new ServiceManager();
                    _configManger = new ConfigManager(_serviceManager, configuration);
                    Configuration = _configManger.Config;
                }
            }
        }

        public static void UpdateConfiguration(ConfigModel configuration = null)
        {
            if (_serviceManager == null)
            {
                Initialize(configuration);
            }
            else
            {
                lock (_initializeLock)
                {
                    _serviceManager.ClearAllWorkers();
                    _configManger = new ConfigManager(_serviceManager, configuration);
                    Configuration = _configManger.Config;
                }
            }
        }

        public static Caller GetCaller(string targetName = null)
        {
            Initialize();
            return _configManger.GetCaller(targetName);
        }
        
        public static List<IStateProvider> GetStateProviders(Type type = null)
        {
            Initialize();
            return _configManger.GetStateProviders(type);
        }

        public static List<IStateProvider> GetStateProviders(Type type = null, string targetName = null)
        {
            Initialize();
            return _configManger.GetStateProviders(type, targetName);
        }

        public static IStateProvider GetStateProvider(string name)
        {
            Initialize();
            return _configManger.GetStateProvider(name);
        }
    }
}

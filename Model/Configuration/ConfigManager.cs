using Datapecker.Agent.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class ConfigManager
    {
        //поля
        private Dictionary<string, TargetContext> _targets;
        private Dictionary<string, Caller> _callers;
        private List<StateProviderConfig> _stateProviderConfigs;
        private ServiceManager _serviceManager;


        //свойства
        public ConfigModel Config { get; set; }



        //инициализация
        public ConfigManager()
        {
            _targets = new Dictionary<string, TargetContext>();
            _callers = new Dictionary<string, Caller>();
            _stateProviderConfigs = new List<StateProviderConfig>();
        }
        public ConfigManager(ServiceManager serviceManager)
            : this()
        {
            _serviceManager = serviceManager;
            Config = LoadFromConfig();
            RegisterDataSources(Config);
        }
        public ConfigManager(ServiceManager serviceManager, ConfigModel config)
            : this()
        {
            if (config != null)
            {
                config.Validate();
            }

            _serviceManager = serviceManager;
            Config = config ?? LoadFromConfig();
            RegisterDataSources(Config);
        }


        
        //загрузка конфига
        private ConfigModel LoadFromConfig()
        {
            ConfigBuildContext context = new ConfigBuildContext();

            DatapeckerSection section = LoadSection(context);
            if (section == null)
            {
                return null;
            }

            List<TargetConfig> targets =
                LoadElements<TargetElement, TargetConfig>(context, section.Targets);

            context.TypeFinder = new TypeFinder<IStateProvider, StateProviderAttribute>();
            List<StateProviderConfig> stateProviders =
                LoadElements<StateProviderElement, StateProviderConfig>(context, section.StateProviders);

            ConfigModel config = new ConfigModel()
            {
                Targets = targets,
                StateProviders = stateProviders
            };

            TypeFinder.ClearAppDomainTypesCache();
            config.Validate(context);

            return config;
        }

        private DatapeckerSection LoadSection(ConfigBuildContext context)
        {
            DatapeckerSection section = null;

            try
            {
                section = (DatapeckerSection)ConfigurationManager.GetSection(AgentConstants.CONFIG_SECTION_NAME);

                if (section == null)
                {
                    string message = string.Format(MessageResources.ConfigManager_NoSectionException
                        , AgentConstants.CONFIG_SECTION_NAME);
                    context.Exceptions.Add(new Exception(message));
                    return null;
                }
            }
            catch (Exception cfgException)
            {
                context.Exceptions.Add(cfgException);
            }

            return section;
        }

        private List<TModel> LoadElements<TElement, TModel>(
            ConfigBuildContext context, ConfigurationElementCollection collection)
            where TElement : ConfigurationElement
            where TModel : ConfigElementBase, new()
        {
            List<TModel> list = new List<TModel>();

            foreach (TElement element in collection)
            {
                TModel item = new TModel();
                item.FillFromConfig(context, element);                
                list.Add(item);
            }

            return list;
        }
        

        

        //регистрация источников данных
        private void RegisterDataSources(ConfigModel config)
        {
            if (config == null)
            {
                return;
            }

            foreach (TargetConfig target in config.Targets)
            {
                TargetContext targetContext = new TargetContext(target);

                _targets.Add(target.Name, targetContext);
                _callers.Add(target.Name, new Caller(targetContext));

                _serviceManager.RegisterTarget(targetContext);
            }
                        
            foreach (StateProviderConfig item in config.StateProviders)
            {
                TargetContext targetContext = _targets[item.TargetName];
                _serviceManager.RegisterStateProvider(targetContext, item.StateProvider);
                
                _stateProviderConfigs.Add(new StateProviderConfig()
                {
                    Name = item.Name,
                    StateProvider = item.StateProvider,
                    TargetName = item.TargetName
                });
            }
        }

      

        //получение
        public Caller GetCaller(string targetName = null)
        {
            ValidateTargetName(targetName);
            targetName = targetName ?? AgentConstants.CONFIG_CATCH_ALL_NAME;              
            return _callers[targetName];
        }
        
        public List<IStateProvider> GetStateProviders(Type type = null)
        {
            IEnumerable<StateProviderConfig> providers = _stateProviderConfigs;

            if (type != null)
            {
                providers = providers.Where(p => p.StateProvider.GetType() == type);
            }

            return providers.Select(p => p.StateProvider).ToList();
        }

        public List<IStateProvider> GetStateProviders(Type type = null, string targetName = null)
        {
            ValidateTargetName(targetName);
            targetName = targetName ?? AgentConstants.CONFIG_CATCH_ALL_NAME;

            IEnumerable<StateProviderConfig> providers = _stateProviderConfigs
                .Where(p => p.TargetName == targetName);

            if (type != null)
            {
                providers = providers.Where(p => p.StateProvider.GetType() == type);
            }

            return providers.Select(p => p.StateProvider).ToList();
        }

        public IStateProvider GetStateProvider(string name)
        {
            StateProviderConfig config = _stateProviderConfigs.FirstOrDefault(p => p.Name == name);

            if (config == null)
            {
                throw new KeyNotFoundException(MessageResources.ConfigManager_StateProviderNotFound);
            }

            return config.StateProvider;
        }

        private void ValidateTargetName(string targetName)
        {
            if (Config == null)
            {
                throw new Exception(
                   string.Format(MessageResources.ConfigManager_ConfigNotInitialized));
            }

            if (targetName == null && !_targets.ContainsKey(AgentConstants.CONFIG_CATCH_ALL_NAME))
            {
                throw new KeyNotFoundException(
                   string.Format(MessageResources.ConfigManager_DefaultTargetNotFound));
            }
            else if (targetName != null && !_targets.ContainsKey(targetName))
            {
                throw new KeyNotFoundException(
                   string.Format(MessageResources.ConfigManager_TargetNotFound, targetName));
            }
        }

        

    }
}

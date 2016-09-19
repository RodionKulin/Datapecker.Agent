using Datapecker.Agent.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    public class StateProviderConfig : ConfigElementBase
    {
        //поля
        private string _typeString;


        //свойства
        public string TargetName { get; set; }
        public IStateProvider StateProvider { get; set; }



        //методы
        internal override void FillFromConfig(ConfigBuildContext context, ConfigurationElement element)
        {
            var stateProviderElement = (StateProviderElement)element;
            
            _typeString = stateProviderElement.Type;

            Name = stateProviderElement.Name;
            TargetName = stateProviderElement.TargetName;
        }

        internal override bool Validate(ConfigBuildContext context)
        {
            bool result = true;
            string elementTypeName = typeof(StateProviderConfig).Name;

            //Name
            if (string.IsNullOrEmpty(Name))
            {
                context.Exceptions.Add(new ArgumentException(
                    string.Format(MessageResources.ConfigModel_ParameterMissing
                    , StateProviderElement.NAME_PARAM, elementTypeName)));
                result = false;
            }

            //Type
            if(StateProvider == null && string.IsNullOrEmpty(_typeString))
            {
                context.Exceptions.Add(new Exception(MessageResources.StateProviderConfig_NotSet));
                result = false;
            }
            else if (StateProvider == null)
            {
                StateProvider = CreateFromType(context, _typeString);
                result = StateProvider == null ? false : result;
            }            

            //TargetName
            if (string.IsNullOrEmpty(TargetName))
            {
                context.Exceptions.Add(new ArgumentException(
                    string.Format(MessageResources.ConfigModel_ParameterMissing
                    , StateProviderElement.TARGET_NAME_PARAM, elementTypeName)));
                result = false;
            }

            return result;
        }

        private IStateProvider CreateFromType(ConfigBuildContext context, string typeString)
        {
            IStateProvider stateProvider = null;
            Type type = context.TypeFinder.Find(typeString);

            if (type == null)
            {
                Exception exception = new Exception(
                    string.Format(MessageResources.StateProviderConfig_TypeNotFound, typeString));
                context.Exceptions.Add(exception);
            }
            else if (!type.GetInterfaces().Contains(typeof(IStateProvider)))
            {
                string interfaceName = typeof(IStateProvider).FullName;
                Exception exception = new Exception(
                    string.Format(MessageResources.StateProviderConfig_TypeWrongInterface, type.FullName, interfaceName));
                context.Exceptions.Add(exception);
            }
            else
            {
                try
                {
                    stateProvider = (IStateProvider)Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    string message = string.Format(MessageResources.StateProviderConfig_ActivationFail, typeString);
                    context.Exceptions.Add(new Exception(message, ex));
                }
            }

            return stateProvider;
        }

    }
}

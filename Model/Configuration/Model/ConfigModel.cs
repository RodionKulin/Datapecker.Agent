using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.Reflection;
using Datapecker.Agent.Resources;

namespace Datapecker.Agent
{
    public class ConfigModel
    {
        //свойства
        public List<TargetConfig> Targets { get; set; }    

        public List<StateProviderConfig> StateProviders { get; set; }
        


        //методы
        internal void Validate(ConfigBuildContext context = null)
        {
            context = context ?? new ConfigBuildContext();

            if (context.Exceptions.Count == 0)
            {
                ValidateConfigModels(context, Targets);
                ValidateConfigModels(context, StateProviders);
            }

            if (context.Exceptions.Count == 0)
            {
                ValidateUniqueNames(context, Targets);
                ValidateUniqueStoragePaths(context);

                ValidateUniqueNames(context, StateProviders);
                ValidateTargetNames(context);
            }

            if (context.Exceptions.Count > 0)
            {
                throw new AggregateException(
                    MessageResources.ConfigManager_ConfigLoadError, context.Exceptions);
            }
        }

        private bool ValidateConfigModels<TModel>(ConfigBuildContext context, List<TModel> configModels)
            where TModel : ConfigElementBase
        {
            foreach (TModel item in configModels)
            {
                bool isValid = item.Validate(context);
                if (!isValid)
                {
                    return false;
                }
            }

            return true;
        }

        private void ValidateUniqueNames<T>(ConfigBuildContext context, List<T> models)
           where T : ConfigElementBase
        {
            if (context.Exceptions.Count > 0)
            {
                return;
            }

            List<string> sameNames = models
                .Select(p => p.Name)
                .GroupBy(p => p)
                .Where(p => p.Count() > 1)
                .Select(p => p.Key)
                .ToList();

            if (sameNames.Count > 0)
            {
                string elementName = models.First().GetType().Name;
                string nameInUse = sameNames.First();

                context.Exceptions.Add(new ArgumentException(
                    string.Format(MessageResources.ConfigManager_ElementNameInUse
                    , elementName, nameInUse)));
            }
        }

        private void ValidateUniqueStoragePaths(ConfigBuildContext context)
        {
            List<string> samePaths = Targets
                .Select(p => p.StorageDirectory.FullName.ToLowerInvariant())
                .GroupBy(p => p)
                .Where(p => p.Count() > 1)
                .Select(p => p.Key)
                .ToList();

            if (samePaths.Count > 0)
            {
                string pathInUse = samePaths.First();

                context.Exceptions.Add(new ArgumentException(
                    string.Format(MessageResources.ConfigManager_StoragePathInUse, pathInUse)));
            }
        }

        private void ValidateTargetNames(ConfigBuildContext context)
        {
            foreach (StateProviderConfig item in StateProviders)
            {
                bool targetExists = Targets.Any(p => p.Name == item.TargetName);
                if (!targetExists)
                {
                    context.Exceptions.Add(new Exception(
                       string.Format(MessageResources.ConfigManager_TargetNotFound, item.TargetName)));
                }
            }
        }

    }
}

using Datapecker.Agent.ReportingService;
using Datapecker.Agent.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    public class TargetConfig : ConfigElementBase
    {
        //свойства
        public string ApplicationID { get; set; }
        public string InstanceID { get; set; }
        public string SecretKey { get; set; }
        public string StoragePath { get; set; }


        //вычисляемые свойства
        internal DirectoryInfo StorageDirectory { get; set; }



        //методы
        internal override void FillFromConfig(ConfigBuildContext context, ConfigurationElement element)
        {
            var targetElement = (TargetElement)element;

            Name = targetElement.Name;
            ApplicationID = targetElement.ApplicationID;
            InstanceID = targetElement.InstanceID;
            SecretKey = targetElement.SecretKey;
            StoragePath = targetElement.StoragePath;
        }

        internal override bool Validate(ConfigBuildContext context)
        {
            bool result = ValidateStoratePath(context);
            string elementTypeName = typeof(TargetConfig).Name;

            //Name
            if (string.IsNullOrEmpty(Name))
            {
                context.Exceptions.Add(new ArgumentException(
                    string.Format(MessageResources.ConfigModel_ParameterMissing
                    , TargetElement.NAME_PARAM, elementTypeName)));
                result = false;
            }

            //ApplicationID
            if (ApplicationID == null
                || !new Regex("^[0-9a-fA-F]{24}$").IsMatch(ApplicationID))
            {
                string exceptionMessage = string.Format(MessageResources.ConfigModel_ParameterMissing
                    , TargetElement.APPLICATIONID_PARAM, elementTypeName)
                    + " " + MessageResources.TargetConfig_AppliationIDRequired;
                context.Exceptions.Add(new ArgumentException(exceptionMessage));
                result = false;
            }

            //SecretKey
            if (string.IsNullOrEmpty(SecretKey))
            {
                context.Exceptions.Add(new ArgumentException(
                    string.Format(MessageResources.ConfigModel_ParameterMissing
                    , TargetElement.SECRET_KEY_PARAM, elementTypeName)));
                result = false;
            }
            
            return result;        
        }
                
        private bool ValidateStoratePath(ConfigBuildContext context)
        {
            if (!string.IsNullOrEmpty(StoragePath))
            {
                bool isValidPath = IsValidRelativePath(StoragePath);
                if (!isValidPath)
                {
                    Exception exception = new ArgumentException(string.Format(MessageResources.ConfigModel_ParameterMissing
                        , TargetElement.STORAGE_PATH_PARAM));
                    context.Exceptions.Add(exception);
                    return false;
                }

                Exception directoryException;
                StorageDirectory = GetStorageDirectory(out directoryException);

                if (directoryException != null)
                {
                    string exceptionMessage = string.Format(MessageResources.ConfigModel_ParameterMissing
                        , TargetElement.STORAGE_PATH_PARAM);
                    Exception exception = new ArgumentException(exceptionMessage, directoryException);
                    context.Exceptions.Add(exception);
                    return false;
                }
            }

            return true;
        }

        internal bool IsValidRelativePath(string path)
        {
            Uri result;
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out result))
            {
                return false;
            }
            else if (!result.IsAbsoluteUri)
            {
                return true;
            }
            else if (result.IsFile)
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        private DirectoryInfo GetStorageDirectory(out Exception exception)
        {
            string input = StoragePath;
            DirectoryInfo storageDirectory = null;

            string storagePath = string.IsNullOrEmpty(input)
                ? AgentConstants.STORAGE_DEFAULT_PATH
                : input.Replace("/", "\\").Trim();

            storagePath = Path.IsPathRooted(storagePath)
                ? storagePath
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, storagePath);

            try
            {
                storageDirectory = new DirectoryInfo(storagePath);
                exception = null;
            }
            catch (Exception directoryException)
            {
                exception = directoryException;
            }

            return storageDirectory;
        }

    }
}

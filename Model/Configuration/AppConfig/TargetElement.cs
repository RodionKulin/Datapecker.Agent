using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class TargetElement : ConfigurationElement
    {
        public const string NAME_PARAM = "name";
        public const string STORAGE_PATH_PARAM = "storagePath";
        public const string APPLICATIONID_PARAM = "applicationID";
        public const string INSTANCE_KEY_PARAM = "instanceID";
        public const string SECRET_KEY_PARAM = "secretKey";



        [ConfigurationProperty(NAME_PARAM, IsRequired = false)]
        public string Name
        {
            get
            {
                return (string)this[NAME_PARAM];
            }
            set
            {
                this[NAME_PARAM] = value;
            }
        }

        [ConfigurationProperty(APPLICATIONID_PARAM, IsRequired = false)]
        public string ApplicationID
        {
            get
            {
                return (string)this[APPLICATIONID_PARAM];
            }
            set
            {
                this[APPLICATIONID_PARAM] = value;
            }
        }

        [ConfigurationProperty(INSTANCE_KEY_PARAM, IsRequired = false)]
        public string InstanceID
        {
            get
            {
                return (string)this[INSTANCE_KEY_PARAM];
            }
            set
            {
                this[INSTANCE_KEY_PARAM] = value;
            }
        }

        [ConfigurationProperty(SECRET_KEY_PARAM, IsRequired = false)]
        public string SecretKey
        {
            get
            {
                return (string)this[SECRET_KEY_PARAM];
            }
            set
            {
                this[SECRET_KEY_PARAM] = value;
            }
        }

        [ConfigurationProperty(STORAGE_PATH_PARAM, IsRequired = false)]
        public string StoragePath
        {
            get
            {
                return (string)this[STORAGE_PATH_PARAM];
            }
            set
            {
                this[STORAGE_PATH_PARAM] = value;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class StateProviderElement : ConfigurationElement
    {
        public const string NAME_PARAM = "name";
        public const string TYPE_PARAM = "type";
        public const string TARGET_NAME_PARAM = "targetName";



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
        
        [ConfigurationProperty(TYPE_PARAM, IsRequired = false)]
        public string Type
        {
            get
            {
                return (string)this[TYPE_PARAM];
            }
            set
            {
                this[TYPE_PARAM] = value;
            }
        }

        [ConfigurationProperty(TARGET_NAME_PARAM, IsRequired = false)]
        public string TargetName
        {
            get
            {
                return (string)this[TARGET_NAME_PARAM];
            }
            set
            {
                this[TARGET_NAME_PARAM] = value;
            }
        }

    }
}

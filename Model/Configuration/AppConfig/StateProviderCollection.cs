using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    [ConfigurationCollection(typeof(StateProviderElement), AddItemName = "stateProvider")]
    internal class StateProviderCollection : ConfigurationElementCollection
    {
        //свойства
        public StateProviderElement this[int idx]
        {
            get { return (StateProviderElement)BaseGet(idx); }
        }



        //методы
        protected override ConfigurationElement CreateNewElement()
        {
            return new StateProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StateProviderElement)element).Name;
        }
    }
}

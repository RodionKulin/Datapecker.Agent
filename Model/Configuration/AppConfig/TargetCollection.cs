using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    [ConfigurationCollection(typeof(TargetElement), AddItemName = "target")]
    internal class TargetCollection : ConfigurationElementCollection
    {   
        //свойства
        public TargetElement this[int idx]
        {
            get { return (TargetElement)BaseGet(idx); }
        }



        //методы
        protected override ConfigurationElement CreateNewElement()
        {
            return new TargetElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TargetElement)element).Name;
        }
    }
}

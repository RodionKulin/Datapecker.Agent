using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Datapecker.Agent
{
    public class StateProviderAttribute
    {
        internal string Name { get; set; }


        //инициализация
        public StateProviderAttribute(string name)
        {
            Name = name;
        }
    }
}

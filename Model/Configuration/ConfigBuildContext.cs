using Datapecker.Agent.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class ConfigBuildContext
    {
        public List<Exception> Exceptions { get; set; }

        public TypeFinder TypeFinder { get; set; }



        //инициализация
        public ConfigBuildContext()
        {
            Exceptions = new List<Exception>();
        }

        
    }
}

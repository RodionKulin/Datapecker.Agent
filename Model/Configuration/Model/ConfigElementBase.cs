using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    public abstract class ConfigElementBase
    {
        //свойства
        public string Name { get; set; }


        //методы
        internal abstract void FillFromConfig(ConfigBuildContext context, ConfigurationElement element);

        internal abstract bool Validate(ConfigBuildContext context);
    }
}

using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Datapecker.Agent
{
    //https://github.com/bspell1/NLogEx/blob/master/NLogEx.Mvc/Config/Config.cs
    internal class DatapeckerSection : ConfigurationSection
    {
        //поля
        [ConfigurationProperty("xmlns")]
        private String Ns1 { get { return null; } }

        [ConfigurationProperty("xmlns:xsi")]
        private String Ns2 { get { return null; } }

        [ConfigurationProperty("xsi:noNamespaceSchemaLocation")]
        private String Ns3 { get { return null; } }



        //свойства
        [ConfigurationProperty("targets")]
        public TargetCollection Targets
        {
            get { return (TargetCollection)base["targets"]; }
        }
              
        [ConfigurationProperty("stateProviders")]
        public StateProviderCollection StateProviders
        {
            get { return (StateProviderCollection)base["stateProviders"]; }
        }
    }
}

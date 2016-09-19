using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    public class StateProviderContext
    {
        public string ApplicationID { get; set; }
        public string InstanceID { get; set; }
        public List<GroupEntry> ApplicationSettings { get; set; }
    }
}

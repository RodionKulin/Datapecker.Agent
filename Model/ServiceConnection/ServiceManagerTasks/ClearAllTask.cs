using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class ClearAllTask : IServiceManagerTask 
    {
        //методы
        public void Perform(List<IServiceWorker> serviceWorkers)
        {
            serviceWorkers.Clear();
        }
    }
}

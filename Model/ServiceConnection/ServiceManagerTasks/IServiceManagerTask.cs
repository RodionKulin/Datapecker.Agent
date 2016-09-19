using System;

namespace Datapecker.Agent
{
    internal interface IServiceManagerTask
    {
        void Perform(global::System.Collections.Generic.List<global::Datapecker.Agent.IServiceWorker> serviceWorkers);
    }
}

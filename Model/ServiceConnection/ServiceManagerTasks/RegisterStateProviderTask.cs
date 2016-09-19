using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class RegisterStateProviderTask : RegisterTargetTask
    {
        //поля
        private IStateProvider _stateProvider;


        //инициализация
        public RegisterStateProviderTask(TargetContext targetContext, IStateProvider stateProvider, IServiceQueries queries)
            : base(targetContext, queries)
        {
            _stateProvider = stateProvider;
        }


        //методы
        public override void Perform(List<IServiceWorker> serviceWorkers)
        {
            ReportStateServiceWorker reporterWorker = 
                RegisterIfNotExists<ReportStateServiceWorker>(serviceWorkers);

            reporterWorker.StateProviders.Add(_stateProvider);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class RegisterTargetTask : IServiceManagerTask
    {
        //поля
        protected TargetContext _targetContext;
        private IServiceQueries _queries;


        //инициализация
        public RegisterTargetTask(TargetContext targetContext, IServiceQueries queries)
        {
            _targetContext = targetContext;
            _queries = queries;
        }


        //методы
        public virtual void Perform(List<IServiceWorker> serviceWorkers)
        {
            RegisterIfNotExists<UpdatesServiceWorker>(serviceWorkers);
            RegisterIfNotExists<ReportStateServiceWorker>(serviceWorkers);
            RegisterIfNotExists<CustomEventServiceWorker>(serviceWorkers);
            RegisterIfNotExists<ExceptionsServiceWorker>(serviceWorkers);
        }

        protected T RegisterIfNotExists<T>(List<IServiceWorker> serviceWorkers)
             where T : IServiceWorker, new()
        {
            T worker = (T)serviceWorkers
                .FirstOrDefault(p => p.Context == _targetContext
                && p.GetType() == typeof(T));

            if (worker == null)
            {
                worker = new T()
                {
                    Context = _targetContext,
                    Queries = _queries
                };
                serviceWorkers.Add(worker);
            }

            return worker;
        }

    }
}

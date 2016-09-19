using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class ReportStateServiceWorker : IServiceWorker
    {
        //свойства
        public TargetContext Context { get; set; }
        public IServiceQueries Queries { get; set; }
        public List<IStateProvider> StateProviders { get; set; }


        //инициализация
        public ReportStateServiceWorker()
        {
            StateProviders = new List<IStateProvider>();
        }


        //методы
        public bool CheckIsReady()
        {
            return Context.IsReportTime;
        }

        public bool CheckReportDataExists()
        {
            return StateProviders.Count > 0;
        }

        public bool Connect()
        {
            List<GroupEntry> customState = CollectCustomStateEntries();

            return Queries.ReportCustomState(Context.Credentials, customState, Context.InternalLogger);
        }



        //сбор данных состояния
        private List<GroupEntry> CollectCustomStateEntries()
        {
            List<GroupEntry> state = new List<GroupEntry>();

            StateProviderContext stateProviderContext = new StateProviderContext()
            {
                ApplicationID = Context.Credentials.ApplicationID.ToString(),
                InstanceID = Context.Credentials.InstanceID,
                ApplicationSettings = Context.AgentSettings.ApplicationSettings.Select(p => new GroupEntry()
                {
                    GroupName = p.GroupName,
                    Key = p.Key,
                    Value = p.Value
                }).ToList()
            };

            foreach (IStateProvider stateProvider in StateProviders)
            {
                try
                {
                    List<GroupEntry> stateEntries = stateProvider.Provide(stateProviderContext);
                    state.AddRange(stateEntries);
                }
                catch (Exception exception)
                {
                    Context.InternalLogger.Exception(ExceptionType.StateProvider, exception);
                }
            }

            return state;
        }

    }
}

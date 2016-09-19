using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class CustomEventServiceWorker : IServiceWorker
    {
        //свойства
        public TargetContext Context { get; set; }
        public IServiceQueries Queries { get; set; }
        

        //методы
        public bool CheckIsReady()
        {
            return Context.IsReportTime && Context.DataReportRequired;
        }

        public bool CheckReportDataExists()
        {
            return Context.CustomEventStorage.CustomEvent_HasContent();
        }

        public bool Connect()
        {
            int queryCount = AgentConstants.CUSTOM_EVENT_CASES_SEND_BLOCK;
            
            do
            {
                List<CustomEventCase> customEvents = Context.CustomEventStorage.CustomEvent_Select(queryCount);
                if (customEvents.Count == 0)
                    return true;
                
                bool completed = Queries.ReportCustomEvents(Context.Credentials, customEvents, Context.InternalLogger);
                if (!completed)
                    return false;
                
                Context.CustomEventStorage.CustomEvent_CutRangeFromStart(customEvents.Count);                
            }
            while (true);
        }

    }
}

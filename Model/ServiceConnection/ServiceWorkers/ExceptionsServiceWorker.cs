using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class ExceptionsServiceWorker : IServiceWorker
    {
        //свойства
        public TargetContext Context { get; set; }
        public IServiceQueries Queries { get; set; }



        //публичные методы
        public bool CheckIsReady()
        {
            return Context.IsReportTime && Context.DataReportRequired;
        }

        public bool CheckReportDataExists()
        {
            bool hasExceptionEvents = Context.ExceptionStorage.ExceptionEvent_HasContent();
            if (hasExceptionEvents)
            {
                return true;
            }

            bool hasExceptionCases = Context.ExceptionStorage.ExceptionCase_HasContent();
            if (hasExceptionCases)
            {
                return true;
            }

            return false;
        }

        public bool Connect()
        {
            bool sendResult = ReportExceptionEvents();
            if (!sendResult)
            {
                return sendResult;
            }
                        
            return ReportExceptionCases();
        }



        //приватные методы
        private bool ReportExceptionEvents()
        {
            do
            {
                ExceptionEvent exceptionEvent = Context.ExceptionStorage.ExceptionEvent_SelectFirst();
                if (exceptionEvent == null)
                    return true;
                
                bool completed = Queries.ReportException(Context.Credentials, exceptionEvent, Context.InternalLogger);
                if(!completed)
                    return false;
                
                Context.ExceptionStorage.ExceptionEvent_CutRangeFromStart(1);                
            } while (true);
        }
        
        private bool ReportExceptionCases()
        {
            int queryCount = AgentConstants.EXCEPTION_CASES_SEND_BLOCK;

            do
            {
                List<ExceptionTime> exceptionCases = Context.ExceptionStorage.ExceptionCase_Select(queryCount);
                if (exceptionCases.Count == 0)
                    return true;
                
                bool completed = Queries.ReportExceptionCases(Context.Credentials, exceptionCases, Context.InternalLogger);
                if (!completed)
                    return false;

                Context.ExceptionStorage.ExceptionCase_CutRangeFromStart(exceptionCases.Count);                
            }
            while (true);            
        }

    }
}

using System.Collections.Generic;
using Datapecker.Agent.ReportingService;

namespace Datapecker.Agent
{
    internal interface IExceptionStorageQueries
    {
        bool ExceptionCase_Append(ExceptionTime exceptionCase, bool logException);
        void ExceptionCase_CutRangeFromStart(int count);
        bool ExceptionCase_HasContent();
        List<ExceptionTime> ExceptionCase_Select(int count);
        bool ExceptionEvent_Append(ExceptionEvent exceptionEvent, bool logException);
        void ExceptionEvent_CutRangeFromStart(int count);
        bool ExceptionEvent_HasContent();
        ExceptionEvent ExceptionEvent_SelectFirst();
        List<string> ExceptionEvent_SelectDistinctKeys(bool logException);
        void KnownExceptionKey_Create(List<string> eventKeys);
        List<string> KnownExceptionKey_Select(bool logException);
    }
}
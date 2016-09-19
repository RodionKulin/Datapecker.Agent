using System.Collections.Generic;
using Datapecker.Agent.ReportingService;

namespace Datapecker.Agent
{
    internal interface IServiceQueries
    {
        void CloseConnection();
        AgentSettings GetAgentSettings(ApplicationCredentials credentials, IInternalLogger logger);
        KnownExceptionsState GetKnownExceptionKeys(ApplicationCredentials credentials, IInternalLogger logger);
        ServerUpdates GetServerUpdates(ApplicationCredentials credentials, AgentState agentState, IInternalLogger logger);
        bool ReportCustomEvents(ApplicationCredentials credentials, List<CustomEventCase> cases, IInternalLogger logger);
        bool ReportException(ApplicationCredentials credentials, ExceptionEvent exceptionEvent, IInternalLogger logger);
        bool ReportExceptionCases(ApplicationCredentials credentials, List<ExceptionTime> exceptionCases, IInternalLogger logger);
        bool ReportCustomState(ApplicationCredentials credentials, List<GroupEntry> customState, IInternalLogger logger);
    }
}
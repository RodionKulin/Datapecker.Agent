using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class UpdatesServiceWorker : IServiceWorker
    {
        //свойства
        public TargetContext Context { get; set; }
        public IServiceQueries Queries { get; set; }

        

        //методы
        public bool CheckIsReady()
        {
            return Context.IsReportTime;
        }

        public bool CheckReportDataExists()
        {
            return true;
        }

        public bool Connect()
        {
            AgentState agentState = CollectAgentState();
            ServerUpdates serverUpdates = Queries.GetServerUpdates(Context.Credentials, agentState, Context.InternalLogger);

            if (serverUpdates == null)
            {
                return false;
            }
            if (serverUpdates.Errors != null && serverUpdates.Errors.Count > 0)
            {
                Context.SettingsStorage.Errors_Create(serverUpdates.Errors);
                return false;
            }

            Context.SettingsStorage.Errors_Delete();
            return CheckUpdates(serverUpdates);
        }



        //приватные методы
        private AgentState CollectAgentState()
        {
            bool isDebug = false;
#if DEBUG
            isDebug = true;
#endif

            return new AgentState()
            {
                IsDebug = isDebug,
                AgentVersion = Assembly.GetAssembly(typeof(ReportStateServiceWorker)).GetName().Version,
                StorageWorking = Context.StoragesState.All(p => p.Value == true)
            };
        }

        private bool CheckUpdates(ServerUpdates serverUpdates)
        {
            bool result = true;

            //настройки приложения
            if (serverUpdates.SettingsToken != Context.AgentSettings.SettingsToken)
            {
                result = GetAgentSettings();

                if (!result)                
                    return result;                
            }


            //токен известных событий исключений
            if (serverUpdates.KnownExceptionEventsToken != Context.ReportingState.KnownExceptionsToken)
            {
                result = GetKnownExceptionKeys();
            }

            return result;
        }
        
        private bool GetAgentSettings()
        {
            AgentSettings settings = Queries.GetAgentSettings(
                Context.Credentials, Context.InternalLogger);

            if (settings != null)
            {
                Context.AgentSettings = settings;
                Context.SettingsStorage.AgentSettings_Create(settings);
            }
            
            return settings != null;
        }
  
        private bool GetKnownExceptionKeys()
        {          
            KnownExceptionsState state = Queries.GetKnownExceptionKeys(
                Context.Credentials, Context.InternalLogger);

            if (state != null)
            {
                Context.ReportingState.KnownExceptionsToken = state.Token;
                Context.ExceptionStorageManager.KnownExceptionEventKey_Rewrite(state.EventKeys);
            }
            
            return state != null;
        }
    }
}

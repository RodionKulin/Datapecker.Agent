using Datapecker.Agent.ReportingService;
using Datapecker.Agent.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class SettingsStorageQueries
    {
        //поля
        private FileStorage _settingsFile;
        private FileStorage _reportStateFile;
        private FileStorage _errorsFile;
        private IInternalLogger _internalLogger;


        //инициализация
        public SettingsStorageQueries(DirectoryInfo folder, IInternalLogger internalLogger)
        {
            string settingsFileName = Path.Combine(folder.FullName, AgentConstants.STORAGE_AGENT_SETTINGS_FILE);
            FileInfo settingsFile = new FileInfo(settingsFileName);
            _settingsFile = new FileStorage(settingsFile);

            string stateFileName = Path.Combine(folder.FullName, AgentConstants.STORAGE_REPORTING_STATE_FILE);
            FileInfo stateFile = new FileInfo(stateFileName);
            _reportStateFile = new FileStorage(stateFile);

            string errorsFileName = Path.Combine(folder.FullName, AgentConstants.STORAGE_ERRORS_FILE);
            FileInfo errorsFile = new FileInfo(errorsFileName);
            _errorsFile = new FileStorage(errorsFile);

            _internalLogger = internalLogger;
        }


        //AgentSettings
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void AgentSettings_Create(AgentSettings settings)
        {
            Exception exception;
            _settingsFile.CreateItem(settings, out exception);
            
            if (exception != null)
            {
                _internalLogger.Exception(ExceptionType.Storage, exception);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public AgentSettings AgentSettings_Select()
        {
            Exception exception;
            AgentSettings agentSettings = _settingsFile.ReadItem<AgentSettings>(out exception);

            if (exception != null)
            {
                _internalLogger.Exception(ExceptionType.Storage
                    , exception, MessageResources.TargetContext_AppSettingsLoadException);
            }

            return agentSettings;
        }

        
        //ReportingState
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ReportingState_Create(ReportingState state)
        {
            Exception exception;
            _reportStateFile.CreateItem(state, out exception);
            
            if (exception != null)
            {
                _internalLogger.Exception(ExceptionType.Storage, exception);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ReportingState ReportingState_Select()
        {
            Exception exception;
            ReportingState reportingState = _reportStateFile.ReadItem<ReportingState>(out exception);

            if (exception != null)
            {
                _internalLogger.Exception(ExceptionType.Storage
                    , exception, MessageResources.TargetContext_AppSettingsLoadException);
            }

            return reportingState;
        }



        //Errors
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Errors_Create(List<string> errors)
        {
            Exception exception;
            _errorsFile.CreateLineList(errors, out exception);

            if (exception != null)
            {
                _internalLogger.Exception(ExceptionType.Storage, exception);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Errors_Delete()
        {
            Exception exception;
            _errorsFile.DeleteFile(out exception);

            if (exception != null)
            {
                _internalLogger.Exception(ExceptionType.Storage, exception);
            }
        }
    }
}

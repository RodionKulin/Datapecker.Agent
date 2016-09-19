using Datapecker.Agent.ReportingService;
using Datapecker.Agent.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class InternalLogger : IInternalLogger
    {
        //поля
        private bool _isDebugMode;
        private ExceptionStorageManager _exceptionManager;

        //свойства
        public ExceptionStorageManager ExceptionManager
        {
            set
            {
                _exceptionManager = value;
            }
        }


        //инициализация
        public InternalLogger()
        {
            string debugModeSetting = ConfigurationManager.AppSettings[AgentConstants.DEBUG_MODE_SETTINGS_KEY];
            _isDebugMode = string.Compare(debugModeSetting, "true", true) == 0;
        }

        
        //Error
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Error(ExceptionType type, string message)
        {
            LogError(type, message);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Error(ExceptionType type, string message, params object[] parameters)
        {
            message = string.Format(message, parameters);
            LogError(type, message);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected void LogError(ExceptionType type, string message)
        {
            if (type != ExceptionType.Connection)
            {
                ExceptionEvent exceptionEvent = ExceptionEventCreator.Create(message, 2);

                bool appendRecursive = type != ExceptionType.ExceptionStorage;
                _exceptionManager.ExceptionEvent_Append(exceptionEvent, appendRecursive);
            }

            if (_isDebugMode)
            {
                throw new Exception(message);
            }
        }


        //Exception
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Exception(ExceptionType type, Exception exception)
        {
            LogException(type, exception, null);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Exception(ExceptionType type, Exception exception, string message)
        {
            LogException(type, exception, message);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Exception(ExceptionType type, Exception exception, string message, params object[] parameters)
        {
            message = string.Format(message, parameters);
            LogException(type, exception, message);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void LogException(ExceptionType type, Exception exception, string message)
        {
            if(type != ExceptionType.Connection)
            {
                ExceptionEvent exceptionEvent = ExceptionEventCreator.Create(exception
                    , comments: message, exceptionStackTraceLevel: 2);

                bool appendRecursive = type != ExceptionType.ExceptionStorage;
                _exceptionManager.ExceptionEvent_Append(exceptionEvent, appendRecursive);
            }

            if (_isDebugMode)
            {
                throw new Exception(MessageResources.InternalCaller_DebugException, exception);
            }
        }


        //Dispose
        public void Dispose()
        {
        }
    }
}

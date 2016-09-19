using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Datapecker.Agent.ReportingService;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reflection;
using Datapecker.Agent.Resources;

namespace Datapecker.Agent
{
    public class Caller : ICaller
    {
        //поля
        private TargetContext _context;

        
                
        //инициализация
        internal Caller( TargetContext targetContext)
        {
            _context = targetContext;
        }

        
        
        //Error
        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual void Error(string message)
        {
            LogError(message);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual void Error(string message, params object[] parameters)
        {
            string fullMessage = string.Format(message, parameters);
            LogError(fullMessage);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual void LogError(string message)
        {
            //2 - level of calling method
            //1 - Error(string message)
            //0 - LogError(string message)
            ExceptionEvent exceptionEvent = ExceptionEventCreator.Create(message, 2);
            _context.ExceptionStorageManager.ExceptionEvent_Append(exceptionEvent, true);
        }


        //Exception
        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual void Exception(Exception exception)
        {
            StackFrame exMethodFrame = null;
            if (exception == null || string.IsNullOrEmpty(exception.StackTrace))
            {
                //1 - level of calling method
                //0 - Exception(Exception exception)
                StackTrace stackTrace = new StackTrace(true);
                exMethodFrame = stackTrace.GetFrame(1);
            }
            else
            {
                StackTrace stackTrace = new StackTrace(exception, true);
                exMethodFrame = stackTrace.GetFrame(0);
            }
            
            string message = null;
            if(exMethodFrame != null)
            {
                MethodBase exMethod = exMethodFrame.GetMethod();
                int line = exMethodFrame.GetFileLineNumber();
                message = string.Format(MessageResources.Caller_ExceptionMethod
                    , exMethod.DeclaringType.FullName, exMethod.Name, line);
            }

            LogException(exception, message);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual void Exception(Exception exception, string message)
        {
            LogException(exception, message);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual void Exception(Exception exception, string message, params object[] parameters)
        {
            string fullMessage = string.Format(message, parameters);
            LogException(exception, fullMessage);
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual void LogException(Exception exception, string message)
        {  
            ExceptionEvent exceptionEvent = ExceptionEventCreator.Create(exception, message, 2);
            _context.ExceptionStorageManager.ExceptionEvent_Append(exceptionEvent, true);
        }



        //CustomEvent
        public virtual void CustomEvent(string eventKey,
            Dictionary<string, string> stringProperties = null,
            Dictionary<string, double> numericProperties = null)
        {
            var eventCase = new CustomEventCase()
            {
                EventKey = eventKey,
                ClientRegisteredTimeUtc = DateTime.UtcNow,
                NumericProperties = numericProperties,
                StringProperties = stringProperties
            };

            bool appended = _context.CustomEventStorage.CustomEvent_Append(eventCase);
            _context.StoragesState[StorageType.CustomEventCases] = appended;

            _context.DataReportRequired = true;
        }
    }
}
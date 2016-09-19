using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Datapecker.Agent;
using System.Runtime.InteropServices;

namespace Datapecker.Agent
{
    internal class ExceptionEventCreator
    {
        //2 - level of calling method
        //1 - public Create(
        //0 - private CreateBase(Exception exception, int exceptionStackTraceLevel)
        private const int CREATEBASE_STACK_TRACE_LEVEL = 2;
        //1 - level of calling method
        //0 - public Create(string comments, int exceptionStackTraceLevel = 0, bool toShortString = true)
        private const int CREATE_STACK_TRACE_LEVEL = 1;

               


        //методы
        /// <summary>
        /// Составить данные об исключении на основе класса Exception и текстового сообщения.
        /// </summary>
        /// <param name="exception">Описание исключения.</param>
        /// <param name="comments">Текстовое сообщение об исключении.</param>
        /// <returns>Данные о месте возникновения и причине исключения.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ExceptionEvent Create(Exception exception, string comments = null
            , int exceptionStackTraceLevel = 0, bool toShortString = true)
        {
            ExceptionEvent exceptionEvent = CreateBase(exception, exceptionStackTraceLevel);
            exceptionEvent.Comments = comments;

            exceptionEvent.SubmitterStackTrace = string.IsNullOrEmpty(exception.StackTrace) 
                ? new StackTrace(CREATE_STACK_TRACE_LEVEL + exceptionStackTraceLevel, true).ToString()
                : exception.StackTrace;
            
            if (exception != null)
            {
                exceptionEvent.ExceptionDetails = ToExceptionDetails(exception, new DetailsIDProvider(), null);
            }
                        
            if (toShortString)
            {
                ToShortStringContent(exceptionEvent);
            }

            return exceptionEvent;
        }

        private static List<ExceptionDetails> ToExceptionDetails(Exception exception, DetailsIDProvider idProvider, int? parentID)
        {
            List<ExceptionDetails> details = new List<ExceptionDetails>();
            if(idProvider.TotalItemsAdded >= AgentConstants.MAX_INNER_EXCEPTIONS
               || exception == null)
            {
                return details;
            }

            ExceptionDetails detail = new ExceptionDetails()
            {
                DetailID = idProvider.GetNext(),
                ParentID = parentID,
                ExceptionType = exception.GetType().ToString(),
                Message = exception.Message
            };

            if (exception.TargetSite != null)
            {
                detail.TargetMetadataToken = exception.TargetSite.MetadataToken;
                detail.TargetDeclaringType = exception.TargetSite.DeclaringType.ToString();
                detail.TargetMethod = exception.TargetSite.Name;
                detail.TargetModule = exception.TargetSite.Module.Name;

                StackTrace stackTrace = new StackTrace(exception, true);
                StackFrame exFrame = stackTrace.GetFrame(0);
                detail.TargetLine = exFrame.GetFileLineNumber();
            }

            details.Add(detail);

            if (exception is AggregateException)
            {
                AggregateException aggregateException = (AggregateException)exception;

                foreach (Exception item in aggregateException.InnerExceptions)
                {
                    List<ExceptionDetails> innerDetails = ToExceptionDetails(item, idProvider, detail.DetailID);
                    details.AddRange(innerDetails);
                }
            }
            else
            {
                List<ExceptionDetails> innerDetails = ToExceptionDetails(exception.InnerException, idProvider, detail.DetailID);
                details.AddRange(innerDetails);
            }

            return details;
        }

        /// <summary>
        /// Составить данные об исключении на основе текстового сообщения.
        /// </summary>
        /// <param name="comments">Текстовое сообщение об исключении.</param>
        /// <param name="exceptionStackTraceLevel">Уровень StackTrace, который вызвал метод логирования. 0 - текущий метод, 1 - предыдущий вызывающий метод и т.д.</param>
        /// <returns>Данные о месте возникновения и причине исключения.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ExceptionEvent Create(string comments
            , int exceptionStackTraceLevel = 0, bool toShortString = true)
        {
            ExceptionEvent exceptionEvent = CreateBase(null, exceptionStackTraceLevel);
            exceptionEvent.Comments = comments;

            int stackTraceCallingLevel = CREATE_STACK_TRACE_LEVEL + exceptionStackTraceLevel;
            exceptionEvent.SubmitterStackTrace = new StackTrace(stackTraceCallingLevel, true).ToString();

            if (toShortString)
                ToShortStringContent(exceptionEvent);

            return exceptionEvent;
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static ExceptionEvent CreateBase(Exception exception, int exceptionStackTraceLevel)
        {
            StackFrame exceptionLevelStackFrame;
            if (exception == null || string.IsNullOrEmpty(exception.StackTrace))
            {
                int stackTraceCallingLevel = CREATEBASE_STACK_TRACE_LEVEL + exceptionStackTraceLevel;
                StackTrace stackTrace = new StackTrace(true);
                exceptionLevelStackFrame = stackTrace.GetFrame(stackTraceCallingLevel);
            }
            else
            {
                StackTrace stackTrace = new StackTrace(exception, true);
                exceptionLevelStackFrame = stackTrace.GetFrame(0);
            }

            string eventKey = CreateKey(exceptionLevelStackFrame);
            int lineNumber = exceptionLevelStackFrame.GetFileLineNumber();
            MethodBase callingMethod = exceptionLevelStackFrame.GetMethod();
            
            return new ExceptionEvent()
            {
                EventKey = eventKey,
                SubmitterModule = callingMethod.Module.Name,
                SubmitterClass = callingMethod.DeclaringType.FullName,
                SubmitterMethod = callingMethod.Name,
                SubmitterLine = lineNumber,
                ClientRegisteredTimeUtc = DateTime.UtcNow
            };
        }

        private static string CreateKey(StackFrame exceptionLevelStackFrame)
        {
            MethodBase callingMethod = exceptionLevelStackFrame.GetMethod();
            Assembly assembly = callingMethod.DeclaringType.Assembly;
            object[] assemblyGuidAttributes = assembly.GetCustomAttributes(typeof(GuidAttribute), true);

            string assemblyID = null;
            int offset = exceptionLevelStackFrame.GetILOffset();

            if (assemblyGuidAttributes.Length > 0)
            {
                var assemblyGuidAttribute = (GuidAttribute)assemblyGuidAttributes[0];

                Guid assemblyGuid = new Guid(assemblyGuidAttribute.Value);
                string encodedGuid = Convert.ToBase64String(assemblyGuid.ToByteArray());
                assemblyID = encodedGuid
                  .Replace("/", "_")
                  .Replace("+", "-")
                  .Substring(0, 22);
            }
            else
            {
                assemblyID = assembly.GetName().Name;
            }

            return string.Format("{0}-{1}-{2}", assemblyID, callingMethod.MetadataToken, offset);
        }

        private static void ToShortStringContent(ExceptionEvent exceptionEvent)
        {
            exceptionEvent.SubmitterModule = exceptionEvent.SubmitterModule.ToShortString();
            exceptionEvent.SubmitterMethod = exceptionEvent.SubmitterMethod.ToShortString();
            exceptionEvent.SubmitterClass = exceptionEvent.SubmitterClass.ToShortString();
            exceptionEvent.Comments = exceptionEvent.Comments.ToShortString();
            exceptionEvent.SubmitterStackTrace = exceptionEvent.SubmitterStackTrace.ToShortString(AgentConstants.MAX_STACKTRACE_LENGTH);


            if (exceptionEvent.ExceptionDetails != null)
            {
                foreach (ExceptionDetails item in exceptionEvent.ExceptionDetails)
                {
                    item.ExceptionType = item.ExceptionType.ToShortString();
                    item.Message = item.Message.ToShortString();
                    item.TargetDeclaringType = item.TargetDeclaringType.ToShortString();
                    item.TargetMethod = item.TargetMethod.ToShortString();
                    item.TargetModule = item.TargetModule.ToShortString();
                }
            }
        }
        

    }
}

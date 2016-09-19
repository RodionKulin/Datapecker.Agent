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
    internal class CustomEventStorageQueries
    { 
        //поля
        private FileStorage _customEventsFile;
        private IInternalLogger _internalLogger;


        //инициализация
        public CustomEventStorageQueries(DirectoryInfo folder, IInternalLogger internalLogger)
        {
            string fullFileName = Path.Combine(folder.FullName, AgentConstants.STORAGE_CUSTOM_EVENTS_FILE);
            FileInfo file = new FileInfo(fullFileName);
            _customEventsFile = new FileStorage(file);

            _internalLogger = internalLogger;
        }



        //методы
        [MethodImpl(MethodImplOptions.NoInlining)]
        public List<CustomEventCase> CustomEvent_Select(int count)
        {
            Exception exception;
            List<CustomEventCase> list = _customEventsFile.ReadBinaryList<CustomEventCase>(count, out exception);

            if (exception != null)
            {
                _internalLogger.Exception(ExceptionType.Storage, exception);
            }

            return list;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool CustomEvent_HasContent()
        {
            Exception exception;
            bool hasContent = _customEventsFile.HasBinaryListContent<CustomEventCase>(out exception);

            if (exception != null)
            {
                _internalLogger.Exception(ExceptionType.Storage, exception);
            }

            return hasContent;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public bool CustomEvent_Append(CustomEventCase customEvent)
        {
            Exception exception;
            var list = new List<CustomEventCase> { customEvent };
            _customEventsFile.AppendToBinaryList(list, out exception);

            if(exception != null)
            {
                _internalLogger.Exception(ExceptionType.Storage
                    , exception, MessageResources.CustomEvents_StorageException);
            }

            return exception == null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void CustomEvent_CutRangeFromStart(int count)
        {
            Exception exception;
            _customEventsFile.CutBinaryListFromStart(count, out exception);
            
            if (exception != null)
            {
                _internalLogger.Exception(ExceptionType.Storage, exception);
            }
        }
    }
}

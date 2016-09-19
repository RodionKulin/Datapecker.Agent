using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Datapecker.Agent
{
    internal class ExceptionStorageQueries : IExceptionStorageQueries
    {
        //поля
        private FileStorage _eventFile;
        private FileStorage _casesFile;
        private FileStorage _knownKeysFile;
        private IInternalLogger _internalLogger;


        //инициализация
        public ExceptionStorageQueries(DirectoryInfo folder, IInternalLogger internalLogger)
        {
            _internalLogger = internalLogger;

            string eventsFullFileName = Path.Combine(folder.FullName, AgentConstants.STORAGE_EXCEPTION_EVENTS_FILE);
            FileInfo eventsFile = new FileInfo(eventsFullFileName);
            _eventFile = new FileStorage(eventsFile);

            string casesFullFileName = Path.Combine(folder.FullName, AgentConstants.STORAGE_CASES_FILE);
            FileInfo casesFile = new FileInfo(casesFullFileName);
            _casesFile = new FileStorage(casesFile);

            string knownKeysFullFileName = Path.Combine(folder.FullName, AgentConstants.STORAGE_KNOWN_KEYS_FILE);
            FileInfo knownKeysFile = new FileInfo(knownKeysFullFileName);
            _knownKeysFile = new FileStorage(knownKeysFile);
        }



        //ExceptionEvent
        public ExceptionEvent ExceptionEvent_SelectFirst()
        {
            Exception exception;
            ExceptionEvent item = _eventFile.ReadBinaryList<ExceptionEvent>(1, out exception)
                .FirstOrDefault();
            
            if (exception != null)
            {
                ExceptionEvent_MoveDamaged();
                _internalLogger.Exception(ExceptionType.ExceptionStorage, exception);
            }

            return item;
        }
        public List<string> ExceptionEvent_SelectDistinctKeys(bool logException)
        {
            Exception exception;
            List<string> list = _eventFile.ReadDistinctExceptionEventKeys(out exception);

            if (exception != null)
            {
                ExceptionEvent_MoveDamaged();

                if(logException)
                    _internalLogger.Exception(ExceptionType.ExceptionStorage, exception);
            }

            return list;
        }
        public bool ExceptionEvent_HasContent()
        {
            Exception exception;
            bool result = _eventFile.HasBinaryListContent<ExceptionEvent>(out exception);

            if (exception != null)
            {
                ExceptionEvent_MoveDamaged();
                _internalLogger.Exception(ExceptionType.ExceptionStorage, exception);
            }

            return result;
        }
        public bool ExceptionEvent_Append(ExceptionEvent exceptionEvent, bool logException)
        {
            var list = new List<ExceptionEvent> { exceptionEvent };

            Exception exception;
            _eventFile.AppendToBinaryList(list, out exception);

            if (exception != null)
            {
                ExceptionEvent_MoveDamaged();

                if(logException)
                    _internalLogger.Exception(ExceptionType.ExceptionStorage, exception);
            }

            return exception == null;
        }
        public void ExceptionEvent_CutRangeFromStart(int count)
        {
            Exception exception;
            _eventFile.CutBinaryListFromStart(count, out exception);

            if (exception != null)
            {
                ExceptionEvent_MoveDamaged();
                _internalLogger.Exception(ExceptionType.ExceptionStorage, exception);
            }
        }
        private void ExceptionEvent_MoveDamaged()
        {
            Exception exception;
            _eventFile.MoveDamaged(out exception);
        }



        //KnownExceptionKey
        public List<string> KnownExceptionKey_Select(bool logException)
        {
            Exception exception;
            List<string> list = _knownKeysFile.ReadLineList(out exception);

            if (exception != null)
            {
                KnownExceptionKey_MoveDamaged();

                if (logException)
                    _internalLogger.Exception(ExceptionType.ExceptionStorage, exception);
            }

            return list;
        }
        public void KnownExceptionKey_Create(List<string> eventKeys)
        {
            Exception exception;
            _knownKeysFile.CreateLineList(eventKeys, out exception);

            if (exception != null)
            {
                KnownExceptionKey_MoveDamaged();
                _internalLogger.Exception(ExceptionType.ExceptionStorage, exception);
            }
        }
        private void KnownExceptionKey_MoveDamaged()
        {
            Exception exception;
            _knownKeysFile.MoveDamaged(out exception);
        }



        //ExceptionCase
        public List<ExceptionTime> ExceptionCase_Select(int count)
        {
            List<ExceptionTime> exceptionTimes = new List<ExceptionTime>();

            Exception exception;
            List<string> lines = _casesFile.ReadLineList(count, out exception);

            if (exception != null)
            {
                ExceptionCase_MoveDamaged();
                _internalLogger.Exception(ExceptionType.ExceptionStorage, exception);
            }

            foreach (string line in lines)
            {
                if(string.IsNullOrEmpty(line))                
                    continue;                

                string[] lineParts = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (lineParts.Length < 2)
                    continue;

                DateTime? utcTime = lineParts[1].ToIso8601DateTime();
                if (utcTime == null)
                    continue;

                ExceptionTime time = new ExceptionTime()
                {
                    EventKey = lineParts[0],
                    UtcTime = utcTime.Value
                };
                exceptionTimes.Add(time);
            }
                      
            return exceptionTimes;
        }
        public bool ExceptionCase_HasContent()
        {
            Exception exception;
            bool result = _casesFile.HasLineListContent(out exception);

            if (exception != null)
            {
                ExceptionCase_MoveDamaged();
                _internalLogger.Exception(ExceptionType.ExceptionStorage, exception);
            }

            return result;
        }
        public bool ExceptionCase_Append(ExceptionTime exceptionCase, bool logException)
        {
            List<string> lines = new List<ExceptionTime>() { exceptionCase }
                .Select(p => string.Format("{0} {1}", p.EventKey, p.UtcTime.ToIso8601String()))
                .ToList();

            Exception exception;
            _casesFile.AppendToLineList(lines, out exception);

            if (exception != null)
            {
                ExceptionCase_MoveDamaged();

                if (logException)
                    _internalLogger.Exception(ExceptionType.ExceptionStorage, exception);
            }

            return exception == null;
        }
        public void ExceptionCase_CutRangeFromStart(int count)
        {
            Exception exception;
            _casesFile.CutLineListFromStart(count, out exception);

            if (exception != null)
            {
                ExceptionCase_MoveDamaged();
                _internalLogger.Exception(ExceptionType.ExceptionStorage, exception);
            }
        }
        private void ExceptionCase_MoveDamaged()
        {
            Exception exception;
            _casesFile.MoveDamaged(out exception);
        }
    }
}

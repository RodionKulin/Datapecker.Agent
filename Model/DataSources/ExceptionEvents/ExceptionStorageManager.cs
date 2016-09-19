using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Datapecker.Agent
{
    internal class ExceptionStorageManager
    {
        //поля
        private IExceptionStorageQueries _eventStorageQueries;
        private TargetContext _context;
        private List<string> _storageExceptionEventKeys;
        private List<string> _knownExceptionEventKeys;

        

        //инициализация
        public ExceptionStorageManager(
            IExceptionStorageQueries eventStorageQueries, TargetContext context)
        {
            _eventStorageQueries = eventStorageQueries;
            _context = context;
        }
        

        //ExceptionEvent
        public void ExceptionEvent_Append(ExceptionEvent exceptionEvent, bool appendRecursive)
        {
            //event
            List<string> knownStorageKeys = ExceptionEvent_SelectKeys(appendRecursive);
            List<string> knownAtServerKeys = KnownExceptionEventKey_Select(appendRecursive);

            if (!knownStorageKeys.Contains(exceptionEvent.EventKey)
                && !knownAtServerKeys.Contains(exceptionEvent.EventKey))
            {
                bool exceptionAppended = _eventStorageQueries.ExceptionEvent_Append(exceptionEvent, appendRecursive);
                _context.StoragesState[StorageType.ExceptionEvents] = exceptionAppended;

                _storageExceptionEventKeys.Add(exceptionEvent.EventKey);
            }

            //case
            ExceptionTime exceptionCase = new ExceptionTime()
            {
                EventKey = exceptionEvent.EventKey,
                UtcTime = DateTime.UtcNow
            };
            bool caseAppended = _eventStorageQueries.ExceptionCase_Append(exceptionCase, appendRecursive);
            _context.StoragesState[StorageType.ExceptionEventCases] = caseAppended;
            
            //connection
            _context.DataReportRequired = true;
        }       
        
        private List<string> ExceptionEvent_SelectKeys(bool logExceptions)
        {
            if (_storageExceptionEventKeys == null)
            {
                _storageExceptionEventKeys = _eventStorageQueries
                    .ExceptionEvent_SelectDistinctKeys(logExceptions);                
            }

            return _storageExceptionEventKeys;
        }


        //KnownExceptionEventKey
        public void KnownExceptionEventKey_Rewrite(List<string> newList)
        {
            _eventStorageQueries.KnownExceptionKey_Create(newList);
            _knownExceptionEventKeys = newList;
        }

        private List<string> KnownExceptionEventKey_Select(bool logExceptions)
        {
            if (_knownExceptionEventKeys == null)
            {
                _knownExceptionEventKeys = _eventStorageQueries
                    .KnownExceptionKey_Select(logExceptions);                
            }

            return _knownExceptionEventKeys;
        }                
    }
}

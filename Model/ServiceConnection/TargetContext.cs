using Datapecker.Agent.ReportingService;
using Datapecker.Agent.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class TargetContext
    {
        //поля
        private long _dataReportRequired;


        //свойства
        public IInternalLogger InternalLogger { get; set; }        
        public ApplicationCredentials Credentials { get; set; }
        public AgentSettings AgentSettings { get; set; }       
        public ReportingState ReportingState { get; private set; }
        public bool DataReportRequired
        {
            get
            {
                return Interlocked.Read(ref _dataReportRequired) == 1;
            }
            set
            {
                long longValue = value ? 1 : 0;
                Interlocked.Exchange(ref _dataReportRequired, longValue);
            }
        }
        

        //Storages
        public ExceptionStorageManager ExceptionStorageManager { get; set; }
        public ExceptionStorageQueries ExceptionStorage { get; set; }
        public CustomEventStorageQueries CustomEventStorage { get; set; }
        public SettingsStorageQueries SettingsStorage { get; set; }
        public Dictionary<StorageType, bool> StoragesState { get; set; }

        
        //вычисляемые свойства
        public bool IsReportTime
        {
            get
            {
                TimeSpan timeFromLastConnect = DateTime.UtcNow - ReportingState.LastReportTimeUtc;
                return timeFromLastConnect >= ReportingState.ReportInterval;
            }
        }
        
        

        //инициализация
        public TargetContext(TargetConfig target)
        {
            StoragesState = new Dictionary<StorageType, bool>();
            DataReportRequired = true;

            Credentials = new ApplicationCredentials()
            {
                ApplicationID = target.ApplicationID,
                InstanceID = string.IsNullOrEmpty(target.InstanceID)
                    ? Environment.MachineName
                    : target.InstanceID,
                SecretKey = target.SecretKey
            };
            
            InitStorage(target);
            RestoreSettingsFromStorage();
        }
        
        private void InitStorage(TargetConfig target)
        {
            InternalLogger internalLogger = new InternalLogger();
            
            ExceptionStorage = new ExceptionStorageQueries(target.StorageDirectory, internalLogger);
            ExceptionStorageManager = new ExceptionStorageManager(ExceptionStorage, this);

            CustomEventStorage = new CustomEventStorageQueries(target.StorageDirectory, internalLogger);

            SettingsStorage = new SettingsStorageQueries(target.StorageDirectory, internalLogger);

            internalLogger.ExceptionManager = ExceptionStorageManager;
            InternalLogger = internalLogger;
        }

        private void RestoreSettingsFromStorage()
        {
            AgentSettings = SettingsStorage.AgentSettings_Select()
                ?? new AgentSettings()
                {
                    ApplicationSettings = new List<ReportingService.GroupEntry>(),
                    SettingsToken = Guid.Empty
                };
            
            ReportingState = SettingsStorage.ReportingState_Select()
                ?? new ReportingState()
                {
                    KnownExceptionsToken = Guid.Empty,
                    LastReportTimeUtc = DateTime.MinValue,
                    ReportInterval = AgentConstants.REPORT_INTERVAL
                };
        }

        
    }
}

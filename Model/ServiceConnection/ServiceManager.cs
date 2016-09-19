using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class ServiceManager : IDisposable
    {
        //поля
        private List<IServiceWorker> _serviceWorkers;
        private Queue<IServiceManagerTask> _tasks;
        private object _tasksLock;
        private NonReentrantTimer _timer;
        private IServiceQueries _queries;
        

        //инициализация
        public ServiceManager()
        {
            _serviceWorkers = new List<IServiceWorker>();
            _tasks = new Queue<IServiceManagerTask>();
            _tasksLock = new object();
            _queries = InitServiceQueries();

            _timer = new NonReentrantTimer(CycleThroughServiceWorkers
                , AgentConstants.SERVICE_MANAGER_TIMER_INTERVAL, false);
            _timer.Start();
        }

        private ServiceQueries InitServiceQueries()
        {
            string serviceEndpointName = ConfigurationManager.AppSettings[AgentConstants.SERVICE_ENDPOINT_NAME_KEY];

            string serviceUriSetting = ConfigurationManager.AppSettings[AgentConstants.SERVICE_URI_OVERRIDE_KEY];            
            Uri serviceUri;
            if (!Uri.TryCreate(serviceUriSetting, UriKind.Absolute, out serviceUri))
            {
                serviceUri = AgentConstants.SERVICE_URI;
            }

            return new ServiceQueries(serviceEndpointName, serviceUri);
        }


        //регистрация       
        public void RegisterTarget(TargetContext targetContext)
        {
            lock (_tasksLock)
            {
                _tasks.Enqueue(new RegisterTargetTask(targetContext, _queries));
            }
        }

        public void RegisterStateProvider(TargetContext targetContext, IStateProvider stateProvider)
        {
            lock (_tasksLock)
            {
                _tasks.Enqueue(new RegisterStateProviderTask(targetContext, stateProvider, _queries));
            }
        }
        
        public void ClearAllWorkers()
        {
            lock (_tasksLock)
            {
                _tasks.Clear();
                _tasks.Enqueue(new ClearAllTask());
            }
        }



        //цикл запуска сервисных рабочих
        private bool CycleThroughServiceWorkers()
        {
            ProcessManagerTasks();
            
            IServiceWorker readyWorker = null;

            do
            {
                readyWorker = _serviceWorkers.FirstOrDefault(p => p.CheckIsReady());
                if (readyWorker != null)
                {
                    ConnectTarget(readyWorker.Context);
                }
            }
            while (readyWorker != null);

            _queries.CloseConnection();

            return _timer.IsStarted;
        }

        private void ProcessManagerTasks()
        {
            while (_tasks.Count > 0)
            {
                IServiceManagerTask task;
                lock (_tasksLock)
                {
                    task = _tasks.Dequeue();
                }

                task.Perform(_serviceWorkers);
            }
        }

        private void ConnectTarget(TargetContext targetToConnect)
        {
            List<IServiceWorker> sameTargetWorkers = _serviceWorkers
                .Where(p => p.Context == targetToConnect)
                .ToList();

            foreach (IServiceWorker worker in sameTargetWorkers)
            {
                if (!worker.CheckIsReady() || !worker.CheckReportDataExists())
                {
                    continue;
                }

                bool connectResult = worker.Connect();
                if (!connectResult)
                {
                    break;
                }
            }

            UpdateReportingState(targetToConnect, sameTargetWorkers);
        }

        private void UpdateReportingState(
            TargetContext targetToConnect, List<IServiceWorker> sameTargetWorkers)
        {
            targetToConnect.ReportingState.LastReportTimeUtc = DateTime.UtcNow;
            targetToConnect.DataReportRequired = sameTargetWorkers
                .Any(p => p.CheckReportDataExists() == true);
            
            targetToConnect.SettingsStorage.ReportingState_Create(targetToConnect.ReportingState);
        }


      
        //IDisposable
        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}

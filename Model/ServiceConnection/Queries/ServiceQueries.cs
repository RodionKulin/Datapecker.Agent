using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class ServiceQueries : IServiceQueries
    {
        //поля
        private WcfSafeCall<IReportService> _wcfProxy;


        //инициализация
        public ServiceQueries(string serviceEndpointName, Uri serviceUri)
        {
            if (!string.IsNullOrEmpty(serviceEndpointName))
            {
                var factory = new ChannelFactory<IReportService>(serviceEndpointName);
                _wcfProxy = new WcfSafeCall<IReportService>(factory);
            }
            else
            {
                NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport, false);
                binding.Security.Message.ClientCredentialType = MessageCredentialType.None;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
                binding.CloseTimeout = TimeSpan.FromSeconds(30);
                binding.OpenTimeout = TimeSpan.FromSeconds(30);
                binding.SendTimeout = TimeSpan.FromMinutes(2);
                binding.ReceiveTimeout = TimeSpan.FromMinutes(2);
                binding.MaxReceivedMessageSize = 105000;
                binding.TransferMode = TransferMode.Buffered;

                EndpointAddress endpointAddress = new EndpointAddress(
                    serviceUri, EndpointIdentity.CreateDnsIdentity(AgentConstants.SERVICE_DNS));

                var factory = new ChannelFactory<IReportService>(binding, endpointAddress);
                _wcfProxy = new WcfSafeCall<IReportService>(factory);
            }
        }
        


        //получение настроек
        public ServerUpdates GetServerUpdates(ApplicationCredentials credentials, AgentState agentState, IInternalLogger logger)
        {
            ServerUpdates updates = null;
            Exception exception = null;

            _wcfProxy.SafeCall((client) =>
            {
                updates = client.GetServerUpdates(credentials, agentState);
            }, out exception);

            if (exception != null)
            {
                logger.Exception(ExceptionType.Connection, exception);
            }

            return updates;
        }

        public AgentSettings GetAgentSettings(ApplicationCredentials credentials, IInternalLogger logger)
        {
            AgentSettings settings = null;
            Exception exception = null;

            _wcfProxy.SafeCall((client) =>
            {
                settings = client.GetAgentSettings(credentials);             
            }, out exception);

            if (exception != null)
            {
                logger.Exception(ExceptionType.Connection, exception);
            }

            return settings;
        }

        public KnownExceptionsState GetKnownExceptionKeys(ApplicationCredentials credentials, IInternalLogger logger)
        {
            KnownExceptionsState state = null;
            Exception exception = null;

            _wcfProxy.SafeCall((client) =>
            {
                state = client.GetKnownExceptionEvents(credentials);
            }, out exception);

            if (exception != null)
            {
                logger.Exception(ExceptionType.Connection, exception);
            }

            return state;
        }


        //отправление данных о событиях
        public bool ReportException(ApplicationCredentials credentials
            , ExceptionEvent exceptionEvent, IInternalLogger logger)
        {
            Exception exception = null;
            bool result = false;

            _wcfProxy.SafeCall((client) =>
            {
                result = client.ReportException(credentials, exceptionEvent);
            }, out exception);

            if (exception != null)
            {
                logger.Exception(ExceptionType.Connection, exception);
            }

            return exception == null && result;
        }
        
        public bool ReportExceptionCases(ApplicationCredentials credentials
            , List<ExceptionTime> exceptionCases, IInternalLogger logger)
        {
            Exception exception = null;
            bool result = false;

            _wcfProxy.SafeCall((client) =>
            {
                result = client.ReportExceptionCases(credentials, exceptionCases);
            }, out exception);

            if (exception != null)
            {
                logger.Exception(ExceptionType.Connection, exception);
            }

            return exception == null && result;
        }
        
        public bool ReportCustomEvents(ApplicationCredentials credentials
            , List<CustomEventCase> cases, IInternalLogger logger)
        {
            Exception exception = null;
            bool result = false;

            _wcfProxy.SafeCall((client) =>
            {
                result = client.ReportCustomEvents(credentials, cases);
            }, out exception);

            if (exception != null)
            {
                logger.Exception(ExceptionType.Connection, exception);
            }

            return exception == null && result;
        }

        public bool ReportCustomState(ApplicationCredentials credentials
            , List<GroupEntry> customState, IInternalLogger logger)
        {
            Exception exception = null;

            List<ReportingService.GroupEntry> groupEntries = customState
                .Select(p => new ReportingService.GroupEntry()
            {
                GroupName = p.GroupName,
                Key = p.Key,
                Value = p.Value
            }).ToList();

            _wcfProxy.SafeCall((client) =>
            {
                client.ReportState(credentials, groupEntries);
            }, out exception);

            if(exception != null)
            {
                logger.Exception(ExceptionType.Connection, exception);
            }

            return exception == null;
        }


        //завершение
        public void CloseConnection()
        {
            _wcfProxy.Close();
        }
    }
}

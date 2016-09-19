using Datapecker.Agent.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;

namespace Datapecker.Agent
{
    internal class WcfSafeCall<TInterface>
        where TInterface : class
    {
        //поля
        private ChannelFactory<TInterface> _factory;
        private readonly object lockObject = new object();
        private TInterface _client;


        //свойства
        public bool CloseEachCall { get; set; }


        //события
        public delegate void WcfSafeCallExceptionHandler(WcfSafeCall<TInterface> wcfSafeCall, WcfSafeCallExceptionArg args);

        public event WcfSafeCallExceptionHandler ConnectionException;

        
        
        //инициализация
        public WcfSafeCall(ChannelFactory<TInterface> factory, bool closeEachCall = false)
        {
            if (!typeof(TInterface).IsInterface)
                throw new Exception(typeof(TInterface) + " is not an interface.");
            
            _factory = factory;            
            CloseEachCall = closeEachCall;
        }
        


        //инициализация соединения
        private void InitConnectionIfNotExist(out Exception initException)
        {
            initException = null;

            IClientChannel clientChannel = (IClientChannel)_client;
            bool isClosed = clientChannel == null ||
                (
                    clientChannel.State != CommunicationState.Opened
                    && clientChannel.State != CommunicationState.Opening
                );

            if (isClosed)
            {
                CleanExistingHandlers();

                _client = _factory.CreateChannel();
                clientChannel = (IClientChannel)_client;
                clientChannel.Faulted += Proxy_Faulted;

                try
                {
                    clientChannel.Open();
                }
                catch (Exception ex)
                {
                    initException = ex;
                    RaiseConnectionException(MessageResources.WcfSafeCall_OpenFailed, ex);
                }
            }
        }
                


        //обработка ошибок
        private void Proxy_Faulted(object sender, EventArgs e)
        {
            RaiseConnectionException(MessageResources.WcfSafeCall_ConnectionFailed);

            if (_client != null)
            {
                CleanExistingHandlers();

                IClientChannel clientChannel = (IClientChannel)_client;
                clientChannel.Abort();
            }
        }

        private void RaiseConnectionException(string message, Exception exception = null)
        {
            if (exception == null)
            {
                exception = new Exception(message);
            }

            WcfSafeCallExceptionArg arg = new WcfSafeCallExceptionArg(exception, message);

            if (ConnectionException != null)
            {
                ConnectionException(this, arg);
            }
        }



        //действия
        public bool SafeCall(Action<TInterface> action)
        {
            Exception exception;
            return SafeCall(action, out exception);
        }

        public bool SafeCall(Action<TInterface> action, out Exception exception)
        {
            bool result = false;
            exception = null;

            InitConnectionIfNotExist(out exception);
            if (exception != null)
                return false;
            
            try
            {
                action.Invoke(_client);
                result = true;
            }
            //первым должны идти FaultException<TInterface>
            //ошибка на сервисе
            catch (FaultException fEx)
            {
                exception = fEx;
                RaiseConnectionException(MessageResources.WcfSafeCall_ServerInternalException, fEx);
                ((IClientChannel)_client).Abort();
            }
            //плохой адрес
            catch (CommunicationException comEx)
            {
                exception = comEx;
                RaiseConnectionException(MessageResources.WcfSafeCall_CommunicationException, comEx);
                ((IClientChannel)_client).Abort();
            }
            //нет ответа
            catch (TimeoutException tEx)
            {
                exception = tEx;
                RaiseConnectionException(MessageResources.WcfSafeCall_TimeoutException, tEx);
                ((IClientChannel)_client).Abort();
            }
            //неизвестная ошибка
            catch (Exception gEx)
            {
                exception = gEx;
                RaiseConnectionException(MessageResources.WcfSafeCall_UnknownException, gEx);
            }
            finally
            {
                if (CloseEachCall)
                {
                    CloseClient();
                }
            }

            return result;
        }




        //завершение
        private void CleanExistingHandlers()
        {
            if (_client != null)
            {
                IClientChannel clientChannel = (IClientChannel)_client;
                clientChannel.Faulted -= Proxy_Faulted;
            }
        }

        private void CloseClient(TimeSpan? closeTimeout = null)
        {
            if (_client == null)
                return;

            lock (lockObject)
            {
                IClientChannel channel = ((IClientChannel)_client);

                if (channel.State == CommunicationState.Faulted)
                {
                    channel.Abort();
                }
                else if (channel.State != CommunicationState.Closed && channel.State != CommunicationState.Closing)
                {
                    try
                    {
                        if (closeTimeout != null)
                            channel.Close(closeTimeout.Value);
                        else
                            channel.Close();
                    }
                    catch (Exception ex)
                    {
                        RaiseConnectionException(MessageResources.WcfSafeCall_CloseException, ex);
                        channel.Abort();
                    }
                }
            }
        }

        public void Close(TimeSpan? closeTimeout = null)
        {
            CleanExistingHandlers();

            CloseClient(closeTimeout);
        }
    }
}

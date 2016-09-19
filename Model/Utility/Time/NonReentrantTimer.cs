using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class NonReentrantTimer : IDisposable
    {
        //константы
        private static TimeSpan DISABLED_TIME_SPAN = TimeSpan.FromMilliseconds(-1);


        //поля
        private Timer _timer;
        private DateTime _lastCallbackStartedUtc;
        private TimeSpan _callbackInterval;
        private Func<bool> _timerCallback;
        
        
        //свойства
        public TimeSpan CallbackInterval
        {
            get { return _callbackInterval; }
            set 
            {
                if (_callbackInterval < TimeSpan.Zero)
                    throw new Exception("Timer interval can not be less then zero");

                _callbackInterval = value; 
            }
        }
        /// <summary>
        /// Начинать отсчёт с момента вызова предыдущего Callback делегата.
        /// При включении запуск производится ровно через выбранный интервал, без погрешности на время выполнения Callback.
        /// По умолчанию true.
        /// </summary>
        public bool IntervalFromCallbackStarted { get; set; }
        public bool IsStarted { get; private set; }


        
        //инициализация
        public NonReentrantTimer(Func<bool> timerCallback, TimeSpan callbackInterval
            , bool intervalFromCallbackStarted = true)
        {
            _timer = new Timer(CallBack);
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            _timerCallback = timerCallback;
            CallbackInterval = callbackInterval;
            IntervalFromCallbackStarted = intervalFromCallbackStarted;
        }
        


        //методы
        protected virtual void CallBack(object state)
        {
            _lastCallbackStartedUtc = DateTime.UtcNow;
            
            //выполнить обратный вызов
            bool continueTimer = _timerCallback();
            IsStarted = continueTimer;
            
            //продолжить
            if (continueTimer)
            {
                //выбрать интервал ожидания
                TimeSpan nextCallbackInterval = GetInterval();

                try
                {
                    _timer.Change(nextCallbackInterval, DISABLED_TIME_SPAN);
                }
                catch (ObjectDisposedException timerHasBeenDisposed)
                {
                }
            }
        }
        /// <summary>
        /// Запустить таймер. Начальный интервал отсчитывается от предыдущего вызова Callback или от времени этого запуска. В зависимости от IntervalFromCallbackFinished.
        /// </summary>     
        public virtual void Start()
        {
            if (IsStarted)
                return;

            TimeSpan nextCallbackInterval = GetInterval();

            Start(nextCallbackInterval);
        }
        /// <summary>
        /// Запустить таймер. Начальный интервал указывается в качестве параметра.
        /// </summary>
        /// <param name="dueTime">Начальный интервал</param>
        public virtual void Start(TimeSpan dueTime)
        {
            if (IsStarted)
                return;

            IsStarted = true;

            try
            {
                _timer.Change(dueTime, DISABLED_TIME_SPAN);
            }
            catch (ObjectDisposedException timerHasBeenDisposed)
            {
            }
        }
        public virtual void Stop()
        {
            IsStarted = false;

            try
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            catch (ObjectDisposedException timerHasBeenDisposed)
            {
            }
        }



        //выбор интервала
        protected virtual TimeSpan GetInterval()
        {
            if (IntervalFromCallbackStarted)
            {
                return GetIntervalFromLastCallbackStarted();
            }
            else
            {
                return _callbackInterval;
            }
        }
        protected virtual TimeSpan GetIntervalFromLastCallbackStarted()
        {
            TimeSpan lastSendInterval = DateTime.UtcNow - _lastCallbackStartedUtc;
            TimeSpan nextCallbackInterval;

            if (lastSendInterval > _callbackInterval)
                nextCallbackInterval = TimeSpan.Zero;
            else
                nextCallbackInterval = _callbackInterval - lastSendInterval;

            return nextCallbackInterval;           
        }



        //Dispose
        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}

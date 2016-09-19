using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal class WcfSafeCallExceptionArg : EventArgs
    {
        //свойства
        public Exception Exception { get; set; }
        public string Message { get; set; }



        //инициализация
        public WcfSafeCallExceptionArg()
        {
        }
        public WcfSafeCallExceptionArg(Exception exception, string message)
        {
            Exception = exception;
            Message = message;
        }
    }
}

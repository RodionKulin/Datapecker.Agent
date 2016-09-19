using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    public interface ICaller
    {    
        void Error(string message);

        void Error(string message, params object[] parameters);

        void Exception(Exception exception);

        void Exception(Exception exception, string message);

        void Exception(Exception exception, string message, params object[] parameters);

        void CustomEvent(string eventKey,
            Dictionary<string, string> stringProperties = null,
            Dictionary<string, double> numericProperties = null);
    }
}

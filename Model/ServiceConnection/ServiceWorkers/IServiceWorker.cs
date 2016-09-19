using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    internal interface IServiceWorker
    {
        TargetContext Context { get; set; }
        IServiceQueries Queries { get; set; }

        bool CheckIsReady();
        bool CheckReportDataExists();
        bool Connect();
    }
}

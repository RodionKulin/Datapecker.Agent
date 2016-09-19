using Datapecker.Agent.ReportingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    public interface IStateProvider
    {
        List<GroupEntry> Provide(StateProviderContext context);
    }
}

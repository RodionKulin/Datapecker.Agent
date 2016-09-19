using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    [DataContract]
    internal class ReportingState
    {
        [DataMember]
        public DateTime LastReportTimeUtc { get; set; }
        [DataMember]
        public TimeSpan ReportInterval { get; set; }
        [DataMember]
        public Guid KnownExceptionsToken { get; set; }
    }
}

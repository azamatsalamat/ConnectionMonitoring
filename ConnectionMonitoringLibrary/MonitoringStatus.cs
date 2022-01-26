using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionMonitoringLibrary
{
    public class MonitoringStatus : IStatus
    {
        public DateTime CurrentDateTime { get; set; }
        public bool Status { get; set; }
        public string DisplayStatus
        {
            get => Status ? "Monitoring started" : "Monitoring stopped";
            set { }
        }
    }
}

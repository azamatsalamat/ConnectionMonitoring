using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionMonitoringLibrary
{
    public class PingStatus : IStatus
    {
        public DateTime CurrentDateTime { get; set; }
        public string Address { get; set; }
        public bool Status { get; set; }
        public string DisplayStatus
        {
            get => Status ? "Connected" : "Not Connected";
            set { }
        }
    }
}

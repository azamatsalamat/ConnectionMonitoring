using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionMonitoringLibrary
{
    public class NtpStatus : IStatus
    {
        public DateTime CurrentDateTime { get; set; }
        public bool DnsStatus { get; set; }
        public string DisplayDnsStatus { get => DnsStatus ? "DNS working" : "DNS not responding"; set { } }
        public bool SendStatus { get; set; }
        public string DisplaySendStatus { get => SendStatus ? "Data sent" : "Data NOT sent"; set { } }
        public bool Status { get; set; }
        public string DisplayStatus { get => Status ? "Data received" : "Data NOT received"; set { } }
    }
}

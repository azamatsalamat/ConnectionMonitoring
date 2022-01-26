using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionMonitoringLibrary
{
    public interface IStatus
    {
        DateTime CurrentDateTime { get; set; }
        bool Status { get; set; }
        string DisplayStatus { get; set; }
    }
}

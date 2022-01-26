using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionMonitoringLibrary
{
    public static class FileProcessor
    {
        public static void WriteLog(List<IStatus> log, string filepath)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Time,Address,Status\n");

            foreach (IStatus pingStatus in log)
            {
                sb.Append(pingStatus.CurrentDateTime.ToString("dd.MM.yyyy HH:mm:ss") + "," + pingStatus.DisplayStatus + "\n");
            }

            File.WriteAllText(filepath, sb.ToString());
            sb.Clear();
        }
    }
}

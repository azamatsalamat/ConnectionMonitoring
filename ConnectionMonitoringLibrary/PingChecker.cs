using System;
using System.Net.NetworkInformation;

namespace ConnectionMonitoringLibrary
{
    public static class PingChecker
    {
        public static bool CheckPing(string url)
        {
            bool available;
            try
            {
                PingReply pingReply;
                using (var ping = new Ping())
                    pingReply = ping.Send(url, 2000);

                available = pingReply.Status == IPStatus.Success;
            }
            catch
            {
                available = false;
            }

            return available;
        }
    }
}

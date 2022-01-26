using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionMonitoringLibrary
{
    public class NtpChecker
    {
        public static bool[] CheckNtp(string ntpServer)
        {
            byte[] ntpData = new byte[48];
            ntpData[0] = 0x1B;

            bool[] output = new bool[3];

            IPAddress[] addresses;

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            try
            {
                addresses = Dns.GetHostEntry(ntpServer).AddressList;
                output[0] = true;
            }
            catch
            {
                return AssignFalse(output, 0);
            }

            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], 123);
                socket.Connect(ipEndPoint);
            }
            catch
            {
                return AssignFalse(output, 1);
            }

            try
            {
                socket.ReceiveTimeout = 3000;
                socket.Send(ntpData);

                output[1] = true;
            }
            catch
            {
                return AssignFalse(output, 1);
            }

            try
            {
                socket.Receive(ntpData);
                socket.Close();

                output[2] = true;
            }
            catch
            {
                return AssignFalse(output, 2);
            }

            return output;
        }

        public static bool[] AssignFalse(bool[] output, int startIndex)
        {
            for (int i = startIndex; i < output.Count(); i++)
            {
                output[i] = false;
            }
            return output;
        }
    }
}

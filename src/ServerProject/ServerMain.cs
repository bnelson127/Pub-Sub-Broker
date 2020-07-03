
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Paycom_Seminar_2020
{
    class ServerMain
    {
        static void Main(string[] args)
        {
            int portNum = 9999;

            bool done = false;

            var listener = new TcpListener(IPAddress.Any, portNum);

            listener.Start();

            while(!done)
            {
                Console.Write("Waiting for connection...");
                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("connection accepted.");
                NetworkStream ns = client.GetStream();

                while(client.Connected)
                {
                    byte[] msg = new byte[1024];
                    ns.Read(msg, 0, msg.Length);
                    Console.WriteLine();
                }
            }

            listener.Stop();
            
        }
    }
}

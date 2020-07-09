
using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Paycom_Seminar_2020
{
    class ClientMain
    {
        static void Main(string[] args)
        {
            
            int portNum = 9999;
            string hostName = "localhost";
            TcpClient tcpClient = new TcpClient(hostName, portNum);
            
            UI ui = new UI(tcpClient);
            //Thread listenThread = new Thread( ()=>listenForMessages(client, tcpClient) );
            //listenThread.Start();
            ui.start();
        }

        public static void listenForMessages(Client client, TcpClient connection)
        {
            byte[] input = new byte[1024];
            while (true)
            {
                Console.WriteLine("HI");
                NetworkStream networkStream = connection.GetStream();
                
                networkStream.Read(input, 0, 1024);
                String dataFromClient = System.Text.Encoding.ASCII.GetString(input);
                Console.WriteLine(" >> " + "From server-"+ dataFromClient);
            }
        }
    }
}

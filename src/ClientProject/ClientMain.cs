
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

            TcpClient primaryTcpClient = new TcpClient(hostName, portNum);
            TcpClient secondaryTcpClient = new TcpClient(hostName, portNum);
            
            Client client = new Client(primaryTcpClient, secondaryTcpClient);
            UI ui = new UI(client);

            MenuSystem menuSystem = new MenuSystem(ui);
            Thread listenThread = new Thread( ()=>listenForNotifications(secondaryTcpClient, client) );
            listenThread.Start();

            menuSystem.start();
        }

        public static void listenForNotifications(TcpClient connection, Client client)
        {
            bool continueRunning = true;
            NetworkStream networkStream = connection.GetStream();

            var writer = new StreamWriter(networkStream);
            byte[] bytes = Encoding.UTF8.GetBytes(ClientMessageEncoder.MESSAGE_LISTENER_CONNECTION);
            networkStream.Write(bytes, 0, bytes.Length);

            while (continueRunning)
            {
                try
                {
                
                    byte[] bytesMsgFromServer = new byte[65536];
                    networkStream.Read(bytesMsgFromServer, 0, 65536);
                    String stringMsgFromServer = System.Text.Encoding.ASCII.GetString(bytesMsgFromServer);
                    String serverResponse = stringMsgFromServer.Trim((char) 0);
                    if (serverResponse.Substring(0,2).Equals(ServerMessageDecoder.MESSAGE_NOTIFICATION))
                    {
                        String topicName = serverResponse.Substring(2);
                        
                        if (client.getUsername() != null)
                        {
                            String[] myTopics = client.requestSubscriptionNames();
                            bool isContained = false;
                            for (int i = 0; i<myTopics.Length; i++)
                            {
                                if (myTopics[i].Equals(topicName))
                                {
                                    isContained = true;
                                }
                            }
                            if (isContained)
                            {
                                Console.WriteLine("You have a new message from "+ topicName);
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    continueRunning = false;
                }
                
                
            }
        }

        public static void autoPublish()
        {
            while(true)
            {
                
            }
        }
    }
}

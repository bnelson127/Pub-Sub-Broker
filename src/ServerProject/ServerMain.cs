using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Xml;

namespace Paycom_Seminar_2020
{
    class Program
    {
        
        static void Main(string[] args)
        {
           try
            {
                //this is a list that all new connections are added to. It is 
                //passed to each new thread so that clients can be notified when they have a new message
                ArrayList connections = new ArrayList();

                int port = loadPortNumber();
                TcpListener server = new TcpListener(IPAddress.Any, port);
                server.Start();

                //these objects are used as locks in each thread to prevent data corruption
                Object profileLock = new Object();
                Object topicLock = new Object();
                
                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                    
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    Thread listenThread = new Thread( ()=>handleClient(client, connections, profileLock, topicLock) );
                    listenThread.Start();

                    
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static int loadPortNumber()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("files/config.xml");
            XmlNode portNode = xmlDoc.SelectSingleNode($"//settings/portNumber");
            String stringPort = portNode.Attributes["value"].Value;
            int intPort = Convert.ToInt32(stringPort);
            return intPort;
        }

        public static void handleClient(TcpClient client, ArrayList connections, Object profileLock, Object topicLock)
        {
            // Buffer for reading data
            byte[] bytes = new byte[1048576];
            string data;
            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            //creates a broker to handle messages
            Broker broker = new Broker(connections, profileLock, topicLock);
            
            int i = stream.Read(bytes, 0, bytes.Length);

            while (i != 0)
            {
                try
                {
                    // Translate data bytes to an ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine(String.Format("Received: {0}", data));

                    //checks if the new connection is for listening for message updates
                    if (data.Substring(0,2).Equals(CommunicationProtocol.MESSAGE_LISTENER_CONNECTION))
                    {
                        connections.Add(client.GetStream());
                    }
                    else
                    {
                        //gets a response to send back to the client
                        data = broker.getResponse(data);
                    }
                    

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine(String.Format("Sent: {0}", data));

                    i = stream.Read(bytes, 0, bytes.Length);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                    i = 0;
                }
                
            }

            // Shutdown and end connection
            client.Close();
        }
    }
}
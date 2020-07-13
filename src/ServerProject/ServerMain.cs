using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace Paycom_Seminar_2020
{
    class Program
    {
        
        static void Main(string[] args)
        {
           try
            {
                ArrayList connections = new ArrayList();
                // set the TcpListener on port 13000
                int port = 9999;
                TcpListener server = new TcpListener(IPAddress.Any, port);

                // Start listening for client requests
                server.Start();

                Object profileLock = new Object();
                Object topicLock = new Object();
                //Enter the listening loop
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    Thread listenThread = new Thread( ()=>handleClient(client, connections, profileLock, topicLock) );
                    listenThread.Start();

                    
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("Hit enter to continue...");
            Console.Read();
        }

        public static void handleClient(TcpClient client, ArrayList connections, Object profileLock, Object topicLock)
        {
            // Buffer for reading data
            byte[] bytes = new byte[1024];
            string data;
            Console.WriteLine("newThread");
            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();
            Broker broker = new Broker(connections, profileLock, topicLock);
            int i;

            // Loop to receive all the data sent by the client.
            i = stream.Read(bytes, 0, bytes.Length);

            while (i != 0)
            {
                try
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine(String.Format("Received: {0}", data));

                    // Process the data sent by the client.
                    if (data.Substring(0,2).Equals(ClientMessageDecoder.MESSAGE_LISTENER_CONNECTION))
                    {
                        connections.Add(client.GetStream());
                    }
                    else
                    {
                        data = broker.getResponse(data);
                        Console.WriteLine(data);
                    }
                    

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine(String.Format("Sent: {0}", data));

                    i = stream.Read(bytes, 0, bytes.Length);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString()+DateTime.Now.ToString());
                    i = 0;
                }
                
            }

            // Shutdown and end connection
            client.Close();
        }
    }
}
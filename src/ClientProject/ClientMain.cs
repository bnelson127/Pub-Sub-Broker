/*
This contains the main method for the program. It also has the code that listens
for incoming message updates, and it has the code that automatically publishes
messages if the user has auto run enabled on any topics.
*/
using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections;
using System.Xml;

namespace Paycom_Seminar_2020
{
    class ClientMain
    {
        public static bool continueRunning = true;
        static void Main(string[] args)
        {
            
            int portNum = loadPortNumber();
            string hostName = loadIpAddress();
            try
            {
                //this tcp client is used to send and receive messages for the user
                TcpClient primaryTcpClient = new TcpClient(hostName, portNum);
                //this tcp client is only used to listen for message notifications from the server
                TcpClient secondaryTcpClient = new TcpClient(hostName, portNum);
                Client client = new Client(primaryTcpClient, secondaryTcpClient);
                UI ui = new UI(client);

                MenuSystem menuSystem = new MenuSystem(ui);

                Thread listenThread = new Thread( ()=>listenForNotifications(secondaryTcpClient, client) );
                listenThread.Start();

                Thread autoSendThread = new Thread( ()=>autoPublish(client) );
                autoSendThread.Start();

                menuSystem.start();
  
            }
            catch(Exception e)
            {
                String goAwayWarning = e.ToString();
                Console.WriteLine("A connection could not be made with the server. Please try again later.");
            }
            finally
            {
                continueRunning = false;
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

        public static String loadIpAddress()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("files/config.xml");
            XmlNode addressNode = xmlDoc.SelectSingleNode($"//settings/ipAddress");
            String address = addressNode.Attributes["value"].Value;
            return address;
        }

        public static void listenForNotifications(TcpClient connection, Client client)
        {
            NetworkStream networkStream = connection.GetStream();

            var writer = new StreamWriter(networkStream);
            byte[] bytes = Encoding.UTF8.GetBytes(CommunicationProtocol.MESSAGE_LISTENER_CONNECTION);
            networkStream.Write(bytes, 0, bytes.Length);

            while (continueRunning)
            {
                try
                {
                
                    byte[] bytesMsgFromServer = new byte[1048576];
                    networkStream.Read(bytesMsgFromServer, 0, 1048576);
                    String stringMsgFromServer = System.Text.Encoding.ASCII.GetString(bytesMsgFromServer);
                    String serverResponse = stringMsgFromServer.Trim((char) 0);
                    if (serverResponse.Substring(0,2).Equals(CommunicationProtocol.MESSAGE_NOTIFICATION))
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
                    String goAwayWarning = e.ToString();
                    
                    if (!client.connectionsWereClosed())
                    {
                        Console.WriteLine("Lost connection to the server. Try quitting and then restarting the application.");
                    }
                    
                    
                    continueRunning = false;
                    client.closeConnections();
                }
                
                
            }
        }

        public static void autoPublish(Client client)
        {
            Random rand = new Random();
            while(continueRunning)
            {
                int sleepSeconds = rand.Next(30,90);
                for (int i = 0; i<sleepSeconds; i++)
                {
                    if (continueRunning)
                    {
                        Thread.Sleep(1000);
                    }
                }
                
                if (continueRunning)
                {
                    try
                    {
                        if (client.getUsername() != null)
                        {
                            String[] topics = client.requestMyTopicNames();
                            ArrayList autos = new ArrayList();
                            foreach (String topic in topics)
                            {
                                if (client.requestAutoRunStatus(topic))
                                {
                                    autos.Add(topic);
                                }
                            }
                            String[] autoTopics = Array.ConvertAll(autos.ToArray(), x => x.ToString());
                            foreach (String topic in autoTopics)
                            {
                                String[] defaultMessages = client.requestDefaultMessages(topic);
                                client.publishMessage(topic, defaultMessages[rand.Next(0,defaultMessages.Length)]);
                            }
                        }

                    }
                    catch(Exception e)
                    {
                        String goAwayWarning = e.ToString();
                        if (!client.connectionsWereClosed())
                        {
                            Console.WriteLine("Lost connection to the server. Try quitting and then restarting the application.");
                        }
                        continueRunning = false;
                        client.closeConnections();
                    }
                }
            }
        }
    }
}

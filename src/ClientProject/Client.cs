using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Paycom_Seminar_2020
{
    class Client
    {
        private String _username { get; }
        private String[] _subscriptionNames{ get; }
        private String[] _topicNames{ get; }
        public Client(String username)
        {
            _username = username;
        }
        public String[] requestAvailableTopics()
        {
            return null;
        }

        public void subscribeToTopic(String topicName)
        {

        }

        public void viewCurrentSubscriptions()
        {

        }

        public void unsubscribeFromTopic()
        {

        }

        public void createTopic()
        {

        }

        public void deleteTopic()
        {

        }

        public void sendMessage()
        {
            
        }

        public void sendServerMessage(String message)
        {
            try
            {
                int portNum = 9999;
                string hostName = "localhost";
                var client = new TcpClient(hostName, portNum);


                NetworkStream ns = client.GetStream();

                var writer = new StreamWriter(ns);

                String msg = "Hello, server.";

                byte[] bytes = Encoding.UTF8.GetBytes(msg);
                ns.Write(bytes, 0, bytes.Length);

                client.Close();
            }
            catch(Exception e)
            {

            }
        }
    }

}
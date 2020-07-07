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

        public void sendServerMessage(String message, TcpClient serverConnection)
        {
            try
            {
                NetworkStream ns = serverConnection.GetStream();

                var writer = new StreamWriter(ns);

                byte[] bytes = Encoding.UTF8.GetBytes(message);
                ns.Write(bytes, 0, bytes.Length);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

}
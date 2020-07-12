using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Collections;

namespace Paycom_Seminar_2020
{
    class Client
    {
        private String _username;
        private TcpClient _primaryTcpClient;
        private TcpClient _secondaryTcpClient;
        public Client(TcpClient primaryTcpClient, TcpClient secondaryTcpClient)
        {
            _primaryTcpClient = primaryTcpClient;
            _secondaryTcpClient = secondaryTcpClient;
        }

        public String[] requestUsernames()
        {
            String nameString = sendServerMessage(ClientMessageEncoder.REQUEST_USERNAMES);
            String[] names = parseString(nameString);
            return names;
        }

        public String[] requestAllTopicNames()
        {
            String stringNames = sendServerMessage(ClientMessageEncoder.REQUEST_TOPIC_NAMES);
            String[] arrayNames = parseString(stringNames);
            return arrayNames;
        }

        public String[] requestNotYetSubscribedTopicNames()
        {
            String stringNames = sendServerMessage(ClientMessageEncoder.REQUEST_NOT_SUBSCRIBED_TOPIC_NAMES);
            String[] arrayNames = parseString(stringNames);
            return arrayNames;
        }

        public String[] requestMyTopicNames()
        {
            String stringNames = sendServerMessage(ClientMessageEncoder.REQUEST_USERS_TOPIC_NAMES);
            String[] arrayNames = parseString(stringNames);
            return arrayNames;
        }

        public String[]  requestSubscriptionNames()
        {
            String stringNames = sendServerMessage(ClientMessageEncoder.REQUEST_SUBSCRIPTION_NAMES);
            String[] arrayNames = parseString(stringNames);
            return arrayNames;
        }

        public String[] requestSubscriptionMessages(String topicName)
        {
            String stringMessages = sendServerMessage(ClientMessageEncoder.REQUEST_SUBSCRIPTION_MESSAGES+topicName);
            String[] arrayMessages = parseString(stringMessages);
            return arrayMessages;
        }

        public void subscribeToTopic(String topicName)
        {
            sendServerMessage(ClientMessageEncoder.ADD_SUBSCRIPTION+topicName);
        }

        public void publishMessage(String topicName, String message)
        {
            String serverResponse = sendServerMessage($"{ClientMessageEncoder.PUBLISH_MESSAGE}{topicName};{message};");
        }

        public String sendServerMessage(String message)
        {
            String serverResponse = "error";
            try
            {
                NetworkStream ns = _primaryTcpClient.GetStream();

                var writer = new StreamWriter(ns);

                byte[] bytes = Encoding.UTF8.GetBytes(message);
                ns.Write(bytes, 0, bytes.Length);

                byte[] bytesMsgFromServer = new byte[65536];
                ns.Read(bytesMsgFromServer, 0, 65536);
                String stringMsgFromServer = System.Text.Encoding.ASCII.GetString(bytesMsgFromServer);
                serverResponse = stringMsgFromServer.Trim((char) 0);
                Console.WriteLine(" >> " + "From server-"+ serverResponse);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            return serverResponse;
        }

        public void setUsername(String username)
        {
            _username = username;
        }

        public String getUsername()
        {
            return _username;
        }

        public void closeConnections()
        {
            _primaryTcpClient.Close();
            _secondaryTcpClient.Close();
        }

        private String[] parseString(String  combinedString)
        {
            ArrayList separatedString = new ArrayList();
            String delimiter = ";";
            String word = "";
            for (int i = 0; i<combinedString.Length; i++)
            {
                if (combinedString.Substring(i, 1).Equals(delimiter) && !word.Equals(""))
                {
                    separatedString.Add(word);
                    word = "";
                }
                else
                {
                    word += combinedString.Substring(i, 1);
                }
            }
            if (!word.Equals("") && !word.Equals(delimiter))
            {
                separatedString.Add(word);
            }
            return Array.ConvertAll(separatedString.ToArray(), x => x.ToString());
            
        }
    }

}
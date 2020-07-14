/*
This class is responsible for communicating with the server. It sends data to the
server and then processes the server's resposne.
*/
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
        private bool _connectionsClosed = false;
        public Client(TcpClient primaryTcpClient, TcpClient secondaryTcpClient)
        {
            _primaryTcpClient = primaryTcpClient;
            _secondaryTcpClient = secondaryTcpClient;
        }

        public String sendServerMessage(String message)
        {
            String serverResponse = ";";
            try
            {
                NetworkStream ns = _primaryTcpClient.GetStream();

                var writer = new StreamWriter(ns);

                byte[] bytes = Encoding.UTF8.GetBytes(message);
                ns.Write(bytes, 0, bytes.Length);

                byte[] bytesMsgFromServer = new byte[1048576];
                ns.Read(bytesMsgFromServer, 0, 1048576);
                String stringMsgFromServer = System.Text.Encoding.ASCII.GetString(bytesMsgFromServer);
                serverResponse = stringMsgFromServer.Trim((char) 0);
            }
            catch(Exception e)
            {
                String goAwayWarning = e.ToString();
            }
            
            return serverResponse;
        }

        public void closeConnections()
        {
            _primaryTcpClient.Close();
            _secondaryTcpClient.Close();
            _connectionsClosed = true;
        }

        public bool connectionsWereClosed()
        {
            return _connectionsClosed;
        }
        public String[] requestSubscriptionNames()
        {
            String stringNames = sendServerMessage(CommunicationProtocol.REQUEST_SUBSCRIPTION_NAMES);
            String[] arrayNames = parseString(stringNames);
            return arrayNames;
        }

        public String[] requestDefaultMessages(String topicName)
        {
            String stringMessages = sendServerMessage(CommunicationProtocol.REQUEST_DEFAULT_MESSAGES+topicName);
            String[] arrayMessages = parseString(stringMessages);
            return arrayMessages;
        }

        public String[] requestMyTopicNames()
        {
            String stringNames = sendServerMessage(CommunicationProtocol.REQUEST_USERS_TOPIC_NAMES);
            String[] arrayNames = parseString(stringNames);
            return arrayNames;
        }
        public String[] requestUsernames()
        {
            String nameString = sendServerMessage(CommunicationProtocol.REQUEST_USERNAMES);
            String[] names = parseString(nameString);
            return names;
        }

        public String requestWelcomeMessage(String topicName)
        {
            String welcomeMessage = sendServerMessage(CommunicationProtocol.REQUEST_WELCOME_MESSAGE+topicName);
            return welcomeMessage;
        }

        public void publishMessage(String topicName, String message)
        {
            String serverResponse = sendServerMessage($"{CommunicationProtocol.PUBLISH_MESSAGE}{topicName};{message};");
        }

        public String getUsername()
        {
            return _username;
        }

        public String[] requestAllTopicNames()
        {
            String stringNames = sendServerMessage(CommunicationProtocol.REQUEST_TOPIC_NAMES);
            String[] arrayNames = parseString(stringNames);
            return arrayNames;
        }

        public String[] requestNotYetSubscribedTopicNames()
        {
            String stringNames = sendServerMessage(CommunicationProtocol.REQUEST_NOT_SUBSCRIBED_TOPIC_NAMES);
            String[] arrayNames = parseString(stringNames);
            return arrayNames;
        }

        public String[] requestSubscriptionMessages(String topicName)
        {
            String stringMessages = sendServerMessage(CommunicationProtocol.REQUEST_SUBSCRIPTION_MESSAGES+topicName);
            String[] arrayMessages = parseString(stringMessages);
            return arrayMessages;
        }

        public int requestNewMessageCount(String topicName)
        {
            String stringCount = sendServerMessage(CommunicationProtocol.REQUEST_NEW_MESSAGE_COUNT+topicName);
            int intCount = Convert.ToInt32(stringCount);
            return intCount;
        }

        public bool requestAutoRunStatus(String topicName)
        {
            String stringStatus = sendServerMessage(CommunicationProtocol.REQUEST_AUTO_RUN_STATUS+topicName);
            if (stringStatus.Equals("true"))
            {
                return true;
            }
            return false;
        }

        public String toggleAutoRun(String topicName)
        {
            String state = sendServerMessage(CommunicationProtocol.TOGGLE_AUTO_RUN+topicName);
            return state;
        }

        public void addDefaultMessage(String topicName, String message)
        {
            sendServerMessage(CommunicationProtocol.ADD_DEFAULT_MESSAGE+topicName+";"+message);
        }

        public int deleteDefaultMessage(String topicName, String message)
        {
            int numMessagesLeft = Convert.ToInt32(sendServerMessage(CommunicationProtocol.DELETE_DEFAULT_MESSAGE+topicName+";"+message));
            return numMessagesLeft;
        }
        public void subscribeToTopic(String topicName)
        {
            sendServerMessage(CommunicationProtocol.ADD_SUBSCRIPTION+topicName);
        }

        public void unsubscribeFromTopic(String topicName)
        {
            sendServerMessage(CommunicationProtocol.REMOVE_SUBSCRIPTION+topicName);
        }

        public String[] requestTopicHistory(String topicName)
        {
            String stringHistory = sendServerMessage(CommunicationProtocol.REQUEST_TOPIC_HISTORY+topicName);
            String[] arrayHistory = parseString(stringHistory);
            return arrayHistory;
        }

        public void setWelcomeMessage(String topicName, String welcomeMessage)
        {
            sendServerMessage(CommunicationProtocol.SET_WELCOME_MESSAGE+topicName+";"+welcomeMessage);
        }

        public void setUsername(String username)
        {
            _username = username;
        }

        private String[] parseString(String  combinedString)
        {
            ArrayList separatedString = new ArrayList();
            String delimiter = ";";
            String word = "";
            for (int i = 0; i<combinedString.Length; i++)
            {
                if (combinedString.Substring(i, 1).Equals(delimiter))
                {
                    if (!word.Equals(""))
                    {
                        separatedString.Add(word);
                        word = "";
                    }
                    
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
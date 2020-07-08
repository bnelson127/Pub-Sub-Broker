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
        private String[] _subscriptionNames;
        private String[] _topicNames;
        private TcpClient _tcpClient;
        public Client(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }

        public void start()
        {
            String nameString = sendServerMessage(ClientMessageEncoder.REQUEST_USERNAMES);
            String[] names = parseString(nameString);
            String serverMessage = UI.selectProfile(names);
            
            bool successful = false;
            while (!successful)
            {
                String serverResponse = sendServerMessage(serverMessage);
                if (serverResponse.Substring(0,2).Equals(ServerMessageDecoder.NAME_TAKEN))
                {
                    Console.Write("That username is already taken. ");
                    serverMessage = UI.askForNewUsername(names);
                    _username = serverMessage.Substring(2);
                }
                else if (serverResponse.Substring(0,2).Equals(ServerMessageDecoder.PROFILE_DELETED))
                {
                    Console.WriteLine("That profile has been delted while you were deciding on a profile.");
                    start();
                    successful = true;
                }
                else if (serverResponse.Substring(0,2).Equals(ServerMessageDecoder.NO_ACTION_REQUIRED))
                {
                    successful = true;
                }
            }
            
            
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

        public String sendServerMessage(String message)
        {
            String serverResponse = "error";
            try
            {
                NetworkStream ns = _tcpClient.GetStream();

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
            if (!word.Equals(""))
            {
                separatedString.Add(word);
            }
            return Array.ConvertAll(separatedString.ToArray(), x => x.ToString());
            
        }
    }

}
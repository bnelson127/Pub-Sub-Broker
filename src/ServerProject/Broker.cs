using System;
using System.Collections;
using System.Net.Sockets;

namespace Paycom_Seminar_2020
{
   
    class Broker
    {
        private ProfilesReaderWriter profReadWrite = null;
        private TopicReaderWriter topReadWrite = null;
        private Profile _userProfile = null;
        private ArrayList _clientConnections = null;
        
        public Broker(ArrayList clientConnections, Object profileLock, Object topicLock)
        {
            _clientConnections = clientConnections;
            profReadWrite = new ProfilesReaderWriter(profileLock);
            topReadWrite = new TopicReaderWriter(topicLock);
        }
        public string getResponse(string message)
        {
            string response = "You probably forgot to restart the server, dummy.";
            string indicator = message.Substring(0,2);
            message = message.Substring(2);
            message = message.Trim((char) 0);

            if (indicator.Equals(CommunicationProtocol.REQUEST_USERNAMES))
            {
                String[] usernamesArray = profReadWrite.getUsernames();
                response = prepareStringArray(usernamesArray);

            }
            else if (indicator.Equals(CommunicationProtocol.LOG_IN))
            {
                response = loadProfile(message);
            }
            else if (indicator.Equals(CommunicationProtocol.CREATE_PROFILE))
            {
                response = createProfile(message);
            }
            else if (indicator.Equals(CommunicationProtocol.CREATE_TOPIC))
            {
                response = _userProfile.addTopic(message);
            }
            else if (indicator.Equals(CommunicationProtocol.REQUEST_TOPIC_NAMES))
            {
                String[] topicNames = topReadWrite.getTopicNames();
                response = prepareStringArray(topicNames)+";";
            }
            else if (indicator.Equals(CommunicationProtocol.ADD_SUBSCRIPTION))
            {
                _userProfile.subscribe(message);
            }
            else if (indicator.Equals(CommunicationProtocol.REQUEST_NOT_SUBSCRIBED_TOPIC_NAMES))
            {
                response = _userProfile.getNotSubscribedTopicNames();
            }
            else if (indicator.Equals(CommunicationProtocol.REQUEST_USERS_TOPIC_NAMES))
            {
                response = _userProfile.getMyTopicNames();
            }
            else if (indicator.Equals(CommunicationProtocol.PUBLISH_MESSAGE))
            {
                String[] names = parseString(message);
                String topicName = names[0];
                String publishedMessage = names[1];
                publishMessage(topicName, publishedMessage);
                
            }
            else if (indicator.Equals(CommunicationProtocol.REQUEST_SUBSCRIPTION_NAMES))
            {
                response = _userProfile.getSubscriptionNames();
            }
            else if (indicator.Equals(CommunicationProtocol.REQUEST_SUBSCRIPTION_MESSAGES))
            {
                response = _userProfile.getSubscriptionMessages(message);
            }
            else if (indicator.Equals(CommunicationProtocol.REQUEST_NEW_MESSAGE_COUNT))
            {
                response = _userProfile.getNewMessageCount(message);
            }
            else if (indicator.Equals(CommunicationProtocol.REQUEST_AUTO_RUN_STATUS))
            {
                String status = topReadWrite.getAutoRun(message);
                response = status;
            }
            else if (indicator.Equals(CommunicationProtocol.REQUEST_DEFAULT_MESSAGES))
            {
                String status = topReadWrite.getDefaultMessages(message);
                response = status;
            }
            else if (indicator.Equals(CommunicationProtocol.TOGGLE_AUTO_RUN))
            {
                String status = topReadWrite.toggleAutoRun(message);
                response = status;
            }
            else if (indicator.Equals(CommunicationProtocol.ADD_DEFAULT_MESSAGE))
            {
                String[] messages = parseString(message);
                topReadWrite.addDefaultMessage(messages[0], messages[1]);
            }
            else if (indicator.Equals(CommunicationProtocol.DELETE_DEFAULT_MESSAGE))
            {
                String[] messages = parseString(message);
                String topicName = messages[0];
                String deletedMessage = messages[1];
                response = deleteDefaultMessage(topicName, deletedMessage);
                
            }
            else if (indicator.Equals(CommunicationProtocol.REMOVE_SUBSCRIPTION))
            {
                _userProfile.unsubscribe(message);
            }
            else if (indicator.Equals(CommunicationProtocol.REQUEST_WELCOME_MESSAGE))
            {
                response = topReadWrite.getWelcomeMessage(message);
            }
            else if (indicator.Equals(CommunicationProtocol.SET_WELCOME_MESSAGE))
            {
                String[] messages = parseString(message);
                String topicName = messages[0];
                String welcomeMessage = messages[1];
                topReadWrite.setWelcomeMessage(topicName, welcomeMessage);
            }
            else if (indicator.Equals(CommunicationProtocol.REQUEST_TOPIC_HISTORY))
            {
                String[] messages = topReadWrite.getTopicMessages(message, 0);
                messages[0] = message;
                messages[1] = "Start of message history";
                response = prepareStringArray(messages);
            }

            return response;
        }

        private String createProfile(String username)
        {
            String responseMessage = "";

            String[] existingUsernames = profReadWrite.getUsernames();
            //makes absolutely sure that the username has not already been taken.
            if (Array.Find(existingUsernames, element => element.Equals(username))==null)
            {
                _userProfile = profReadWrite.createNewProfile(username);
                _userProfile.setWriters(profReadWrite, topReadWrite);
                responseMessage = CommunicationProtocol.NO_ACTION_REQUIRED+"Profile successfully created.";
            }
            else
            {
                responseMessage = CommunicationProtocol.NAME_TAKEN+"Sorry, that username was taken while you were deciding.";
            }

            return responseMessage;

        }

        private String loadProfile(String profileName)
        {
            String response = "";
            String[] existingUsernames = profReadWrite.getUsernames();
            if (Array.Find(existingUsernames, element => element.Equals(profileName))!=null)
            {
                _userProfile = profReadWrite.loadProfile(profileName);
                _userProfile.setWriters(profReadWrite, topReadWrite);
                response = CommunicationProtocol.NO_ACTION_REQUIRED+"Successfully logged in.";
            }
            else
            {
                response = CommunicationProtocol.PROFILE_DELETED;
            }
            return response;
        }

        private void publishMessage(String topicName, String message)
        {
            topReadWrite.publishMessage(topicName, message);
            for (int i = 0; i<_clientConnections.Count; i++)
            {
                try
                {
                    NetworkStream currentClient = (NetworkStream) _clientConnections[i];
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(CommunicationProtocol.MESSAGE_NOTIFICATION+topicName);
                    currentClient.Write(msg, 0, msg.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                
            }
        }

        private String deleteDefaultMessage(String topicName, String message)
        {
            String stringMessages = topReadWrite.getDefaultMessages(topicName);
            String[] arrayMessages = parseString(stringMessages);
            String newMessages = ";";
            int messageCount = 0;
            foreach (String msg in arrayMessages)
            {
                if (!msg.Equals(message))
                {
                    newMessages+=msg+";";
                    messageCount++;
                }
            }
            topReadWrite.setDefaultMessages(topicName, newMessages);
            if (messageCount == 0 && topReadWrite.getAutoRun(topicName).Equals("true"))
            {
                topReadWrite.toggleAutoRun(topicName);
            }
            String response = messageCount.ToString();
            return response;
        }

        private String prepareStringArray(String[] array)
        {
            String finished = "";
            for (int i = 0; i<array.Length; i++)
            {
                finished+=array[i]+";";
            }
            return finished;
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
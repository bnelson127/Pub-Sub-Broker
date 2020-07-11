using System;
using System.Xml;
using System.Collections;

namespace Paycom_Seminar_2020
{
   
    class Broker
    {
        private ProfilesReaderWriter profReadWrite = new ProfilesReaderWriter();
        private TopicReaderWriter topReadWrite = new TopicReaderWriter();
        private Profile _userProfile = null;
        public string getResponse(string message)
        {
            string response = "You probably forgot to restart the server, dummy.";
            string indicator = message.Substring(0,2);
            message = message.Substring(2);
            message = message.Trim((char) 0);

            if (indicator.Equals(ClientMessageDecoder.REQUEST_USERNAMES))
            {
                String[] usernamesArray = profReadWrite.getUsernames();
                response = prepareStringArray(usernamesArray);

            }
            else if (indicator.Equals(ClientMessageDecoder.LOG_IN))
            {
                String[] existingUsernames = profReadWrite.getUsernames();
                if (Array.Find(existingUsernames, element => element.Equals(message))!=null)
                {
                    _userProfile = profReadWrite.loadProfile(message);
                    response = ServerMessageEncoder.NO_ACTION_REQUIRED+"Successfully logged in.";
                }
                else
                {
                    response = ServerMessageEncoder.PROFILE_DELETED;
                }
                
            }
            else if (indicator.Equals(ClientMessageDecoder.CREATE_PROFILE))
            {
                response = createProfile(message);
            }
            else if (indicator.Equals(ClientMessageDecoder.CREATE_TOPIC))
            {
                response = createTopic(message);
            }
            else if (indicator.Equals(ClientMessageDecoder.REQUEST_TOPIC_NAMES))
            {
                String[] topicNames = topReadWrite.getTopicNames();
                response = prepareStringArray(topicNames)+";";
            }
            else if (indicator.Equals(ClientMessageDecoder.ADD_SUBSCRIPTION))
            {
                profReadWrite.addSubscription(_userProfile.getUsername(), message);
                response = ServerMessageEncoder.NO_ACTION_REQUIRED;
            }
            else if (indicator.Equals(ClientMessageDecoder.REQUEST_NOT_SUBSCRIBED_TOPIC_NAMES))
            {
                String[] topicNames = topReadWrite.getTopicNames();
                ArrayList subNames = profReadWrite.getSubscriptions(_userProfile.getUsername());
                ArrayList filteredList = new ArrayList();
                for (int i = 0; i<topicNames.Length; i++)
                {
                    if (!subNames.Contains(topicNames[i]))
                    {
                        filteredList.Add(topicNames[i]);
                    }
                }

                String[] stringFiltered = Array.ConvertAll(filteredList.ToArray(), x => x.ToString());
                response = prepareStringArray(stringFiltered)+";";
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
                responseMessage = ServerMessageEncoder.NO_ACTION_REQUIRED+"Profile successfully created.";
            }
            else
            {
                responseMessage = ServerMessageEncoder.NAME_TAKEN+"Sorry, that username was taken while you were deciding.";
            }

            return responseMessage;

        }

        private String createTopic(String name)
        {
            String responseMessage = "";

            String[] existingTopicNames = topReadWrite.getTopicNames();
            //makes absolutely sure that the topicname has not already been taken.
            if (Array.Find(existingTopicNames, element => element.Equals(name))==null)
            {
                topReadWrite.createNewTopic(name);
                responseMessage = ServerMessageEncoder.NO_ACTION_REQUIRED+"Topic successfully created.";
                _userProfile.addTopic(name);
            }
            else
            {
                responseMessage = ServerMessageEncoder.NAME_TAKEN+"Sorry, that topic name was taken while you were deciding.";
            }

            return responseMessage;
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
    }

}
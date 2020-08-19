/*
This class is responsible for getting and processing data that comes from the
user's profile. If it needs to read in data from profile.xml or topics.xml it
uses a ProfileReaderWriter or a TopicReaderWriter.
*/

using System;
using System.Collections;

namespace Paycom_Seminar_2020
{
   
    class Profile
    {
        private String _username;
        private ProfilesReaderWriter _profReadWrite = null;
        private TopicReaderWriter _topReadWrite = null;
        public Profile(String username)
        {
            _username = username;
        }

        public void setWriters(ProfilesReaderWriter profReadWrite, TopicReaderWriter topReadWrite)
        {
            _profReadWrite = profReadWrite;
            _topReadWrite = topReadWrite;
        }

        public String getUsername() // I don't think this method is ever used
        {
            return _username;
        }

        public String addTopic(String topicName)
        {
             String responseMessage = "";

            String[] existingTopicNames = _topReadWrite.getTopicNames();
            //makes absolutely sure that the topicname has not already been taken.
            if (Array.Find(existingTopicNames, element => element.Equals(topicName))==null)
            {
                _topReadWrite.createNewTopic(topicName);
                responseMessage = CommunicationProtocol.NO_ACTION_REQUIRED+"Topic successfully created.";
                _profReadWrite.addTopic(_username, topicName);
            }
            else
            {
                responseMessage = CommunicationProtocol.NAME_TAKEN+"Sorry, that topic name was taken while you were deciding.";
            }

            return responseMessage;
            
        }

        public String getNotSubscribedTopicNames()
        {
            String[] topicNames = _topReadWrite.getTopicNames();
            ArrayList subNames = _profReadWrite.getSubscriptions(_username);
            ArrayList filteredList = new ArrayList();
            for (int i = 0; i<topicNames.Length; i++) // good place for a foreach or Linq ForEach
            {
                if (!subNames.Contains(topicNames[i]))
                {
                    filteredList.Add(topicNames[i]);
                }
            }

            String[] stringFiltered = Array.ConvertAll(filteredList.ToArray(), x => x.ToString());

            //semicolon is tacked onto the end to ensure an empty string is not sent
            String response = prepareStringArray(stringFiltered)+";";
            return response;
        }

        public String getMyTopicNames()
        {
            String[] arrayNames = _profReadWrite.getTopics(_username);
            String  stringNames = prepareStringArray(arrayNames);
            String response = stringNames+";";
            return response;
        }

        public String getSubscriptionNames()
        {
            ArrayList subscriptions = _profReadWrite.getSubscriptions(_username);
            String[] arraySubscriptions = Array.ConvertAll(subscriptions.ToArray(), x => x.ToString());
            String stringSubscriptions = prepareStringArray(arraySubscriptions);
            String response = stringSubscriptions+";";
            return response;
        }

        public String getSubscriptionMessages(String topicName)
        {
            long joinTime = _profReadWrite.getSubscriptionDate(_username, topicName);
            String[] messages = _topReadWrite.getTopicMessages(topicName, joinTime);
            messages[1] = _topReadWrite.getWelcomeMessage(topicName);
            _profReadWrite.updateLastChecked(_username, topicName);
            String response = prepareStringArray(messages);
            return response;
        }

        public String getNewMessageCount(String topicName)
        {
            long[] times = _topReadWrite.getMessageTimeStamps(topicName);
            long lastChecked = _profReadWrite.getLastChecked(_username, topicName);
            int newMessageCount = 0;

            foreach(long time in times)
            {
                if (time>lastChecked)
                {
                    newMessageCount++;
                }
            }
            String response = newMessageCount.ToString();
            return response;
        }

        public String subscribe(String topicName)
        {
            _profReadWrite.addSubscription(_username, topicName);
            String response = CommunicationProtocol.NO_ACTION_REQUIRED;
            return response;
        }
        public void unsubscribe(String topicName)
        {
            _profReadWrite.removeSubscription(_username, topicName);
        }

        private String prepareStringArray(String[] array) // Is there a more descriptive name than array?
        {
            String finished = "";
            for (int i = 0; i<array.Length; i++) // could be a one-liner, too: array.ForEach(a => finished += a + ";";);
            {
                finished+=array[i]+";";
            }
            return finished;
        }
    }

}
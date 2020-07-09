using System;

namespace Paycom_Seminar_2020
{
   
    class Profile
    {
        private String _username;
        private String[] _subscriptions;
        private String[] _topics;
        private ProfilesReaderWriter _profReadWrite = new ProfilesReaderWriter();
        public Profile(String username, String[] subscriptions, String[] topics)
        {
            _username = username;
            _subscriptions = subscriptions;
            _topics = topics;
        }

        public String getUsername()
        {
            return _username;
        }

        public void addTopic(String topicName)
        {
            _profReadWrite.addTopic(_username, topicName);
        }
    }

}
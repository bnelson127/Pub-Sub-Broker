using System;

namespace Paycom_Seminar_2020
{
   
    class Profile
    {
        private String _username;
        private String[] _subscriptions;
        private String[] _topics;
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
    }

}
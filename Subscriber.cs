using System;

namespace Paycom_Seminar_2020
{
    class Subscriber
    {
        static void Main(string[] args)
        {
            UI ui = new UI();
            String[] options = {"BOfrdgthyjuB", "BILL", "BILLY", "BOBBY"};
            ui.getMenuResponseWithBackOption(options);
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
    }

}
/*
This class stores all of the indicators that the server and client use to
communicate with one another. 
*/

namespace Paycom_Seminar_2020
{
    static class CommunicationProtocol // this can be an internal static class
    {
        // Looks good.
        public static string REQUEST_USERNAMES {get;} = "00";
        public static string LOG_IN {get;} = "01";
        public static string CREATE_PROFILE {get;} = "02";
        public static string CREATE_TOPIC {get;} = "03";
        public static string REQUEST_TOPIC_NAMES {get;} = "04";
        public static string ADD_SUBSCRIPTION {get;} = "05";
        public static string REQUEST_NOT_SUBSCRIBED_TOPIC_NAMES {get;} = "06";
        public static string REQUEST_USERS_TOPIC_NAMES {get;} = "07";
        public static string PUBLISH_MESSAGE {get;} = "08";
        public static string MESSAGE_LISTENER_CONNECTION {get;} = "09";
        public static string REQUEST_SUBSCRIPTION_NAMES {get;} = "10";
        public static string REQUEST_SUBSCRIPTION_MESSAGES {get;} = "11";
        public static string REQUEST_NEW_MESSAGE_COUNT {get;} = "12";
        public static string REQUEST_AUTO_RUN_STATUS {get;} = "13";
        public static string REQUEST_DEFAULT_MESSAGES {get;} = "14";
        public static string TOGGLE_AUTO_RUN {get;} = "15";
        public static string ADD_DEFAULT_MESSAGE {get;} = "16";
        public static string DELETE_DEFAULT_MESSAGE {get;} = "17";
        public static string REMOVE_SUBSCRIPTION {get;} = "18";
        public static string REQUEST_WELCOME_MESSAGE {get;} = "19";
        public static string SET_WELCOME_MESSAGE {get;} = "20";
        public static string REQUEST_TOPIC_HISTORY {get;} = "21";
        public static string NO_ACTION_REQUIRED {get;} = "22";
        public static string NAME_TAKEN {get;} = "23";
        public static string PROFILE_DELETED {get;} = "24";
        public static string MESSAGE_NOTIFICATION {get;} = "25";
    }

}
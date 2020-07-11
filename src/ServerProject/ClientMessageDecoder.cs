namespace Paycom_Seminar_2020
{
   
    static class ClientMessageDecoder
    {
        public static string REQUEST_USERNAMES {get;} = "00";
        public static string LOG_IN {get;} = "01";
        public static string CREATE_PROFILE {get;} = "02";
        public static string CREATE_TOPIC {get;} = "03";
        public static string REQUEST_TOPIC_NAMES {get;} = "04";
        public static string ADD_SUBSCRIPTION {get;} = "05";
        public static string REQUEST_NOT_SUBSCRIBED_TOPIC_NAMES {get;} = "06";
    }

}
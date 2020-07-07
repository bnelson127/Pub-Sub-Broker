namespace Paycom_Seminar_2020
{
   
    static class Broker
    {
        public static string getResponse(string message)
        {
            string response = null;

            ClientMessageDecoder clientMsgCodes = new ClientMessageDecoder();
            if (message.Substring(0,1).Equals(clientMsgCodes.REQUEST_USERNAMES))
            {
                response = getUsernames();
            }

            return response;
        }

        private static string getUsernames()
        {
            return "Bob;Julie;Eric;Caleb;Stacy";
        }
    }

}
using System;

namespace Paycom_Seminar_2020
{
   
    static class Broker
    {
        public static string getResponse(string message)
        {
            string response = "dummy";
            string indicator = message.Substring(0,2);
            message = message.Substring(2);

            if (indicator.Equals(ClientMessageDecoder.REQUEST_USERNAMES))
            {
                response = getUsernames();
            }
            else if (indicator.Equals(ClientMessageDecoder.LOG_IN))
            {
                Console.WriteLine(message);
            }

            return response;
        }

        private static string getUsernames()
        {
            return "Bob;Julie;Eric;Caleb;Stacy";
        }
    }

}
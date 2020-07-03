
using System;

namespace Paycom_Seminar_2020
{
    class ClientMain
    {
        static void Main(string[] args)
        {
            String username = UI.askQuestionFreeResponse("What is your username?");

            Client client = new Client(username);
            String[] options = new String[] {"View Subscrptions", "Topic Settings", "Subscription Settings", "Quit"};
            UI.getMenuResponse(options);
        }
    }
}

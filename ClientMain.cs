
using System;

namespace Paycom_Seminar_2020
{
    class ClientMain
    {
        static void Main(string[] args)
        {
            String[] options = {"Publisher", "Subscriber"};
            String username = UI.askQuestionFreeResponse("What is your username?");
            int pubOrSub = UI.askQuestionMultipleChoice( "Do you want to publish or subscribe?", options);

            Client client = new Client(username);
        }
    }
}

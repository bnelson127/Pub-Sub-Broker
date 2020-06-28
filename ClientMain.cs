
using System;

namespace Paycom_Seminar_2020
{
    class ClientMain
    {
        static void Main(string[] args)
        {
            UI ui = new UI();
            String[] options = {"Publisher", "Subscriber"};
            String username = ui.askQuestionFreeResponse("What is your username?");
            int pubOrSub = ui.askQuestionMultipleChoice( "Are you a publisher or subscriber?", options);
        }
    }
}

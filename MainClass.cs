
using System;

namespace Paycom_Seminar_2020
{
    class MainClass
    {
        static void Main(string[] args)
        {
            UI ui = new UI();
            String[] options = {"BOB", "BILL", "BILLY", "BOBBY"};
            ui.getMenuResponseWithBackOption(options);
        }
    }
}

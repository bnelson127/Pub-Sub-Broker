/*
This class provides basic menu functionality. When the menu is displayed, it shows
all the options it has been given and then returns the option that the user chose.
*/

using System;

namespace Paycom_Seminar_2020
{
    class Menu
    {
        private String[] _options = null;
        private String _name = null;
        public Menu(String name, String[] options, bool hasBackOption)
        {
            _name = name;
            if (!hasBackOption)
            {
                _options = options;
            }
            else
            {
                _options = new String[options.Length+1];
                for (int i = 0; i<options.Length; i++)
                {
                    _options[i] = options[i];
                }
                _options[_options.Length-1] = "Back";
            }
        }

        public String displayMenu()
        {
            printTitle();

            for (int i = 0; i<_options.Length; i++)
            {
                Console.WriteLine($"[{i+1}]: {_options[i]}"); // Good use of string interpolation
            }

            bool successful = false;
            int intResponse = 0;
            while(!successful)
            {
                try
                {
                    intResponse = Convert.ToInt32(Console.ReadLine());

                    //validates the response
                    // Chunks of logic like this are best to be split off in a function with a clear name in my opinion,
                    // e.g. ValidateResponse()
                    if (intResponse <= 0 || intResponse>_options.Length)
                    {
                        throw new ArgumentException($"{intResponse} is not an integer");
                    }

                    successful = true;
                }
                catch (Exception e)
                {
                    String goAwayWarning = e.ToString();
                    Console.WriteLine("Oops, that's not an option! Enter the number of the seleciton you want to make!");
                }
            }
            
            String stringResponse = _options[intResponse-1];

            return stringResponse;
        }

        private void printTitle()
        {
            Console.WriteLine("");
            printDecorativeLines();
            Console.Write(" ");
            Console.Write(_name.ToUpper());
            Console.WriteLine(" ");
            printDecorativeLines();
        }
        private void printDecorativeLines()
        {
            for (int i = 0; i<_name.Length+2; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine("");
        }
    }

}
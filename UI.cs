using System;

namespace Paycom_Seminar_2020
{
    class UI
    {
        public int getMenuResponse(String[] options)
        {
            for (int i = 0; i<options.Length; i++)
            {
                Console.WriteLine($"[{i+1}]: {options[i]}");
            }

            int input = 0;
            bool successful = false;
            while (!successful)
            {
                Console.WriteLine("Please enter the number of the selection you would like to make.");
                try
                {
                    input = Convert.ToInt32(Console.ReadLine());

                    //validates the input
                    if (input <= 0 || input>options.Length)
                    {
                        throw new ArgumentException("That is out of range");
                    }

                    successful = true;
                }
                catch (Exception e)
                {
                    Console.Write("Oops, that's not an option! ");
                }
                
            }
            Console.WriteLine($"You selected {options[input-1]}");
            
            return 0;
            
        }

        public int getMenuResponseWithBackOption(String[] options)
        {
            String[] newOptions = new String[options.Length+1];
            for (int i = 0; i<options.Length; i++)
            {
                newOptions[i] = options[i];
            }
            newOptions[newOptions.Length-1] = "Back";
            return getMenuResponse(newOptions);
        }
    }

}
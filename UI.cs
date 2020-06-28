using System;

namespace Paycom_Seminar_2020
{
    class UI
    {
        public int getMenuResponse(String[] options)
        {
            Console.WriteLine("Please enter the number of the selection you would like to make.");
            int response = getOptionsResponse(options);
            Console.WriteLine($"You selected {options[response-1]}");
            
            return response;
            
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

        public String askQuestionFreeResponse(String question)
        {
            String response = null;
            Console.WriteLine(question);
            response = Console.ReadLine();

            return response;
        }

        public int askQuestionMultipleChoice(String question, String[] choices)
        {
            Console.Write(question);
            int response = getOptionsResponse(choices);
            return response;
        }

        private int getOptionsResponse(String[] options)
        {
            for (int i = 0; i<options.Length; i++)
            {
                Console.WriteLine($"[{i+1}]: {options[i]}");
            }

            bool successful = false;
            int response = 0;
            while(!successful)
            {
                try
                {
                    response = Convert.ToInt32(Console.ReadLine());

                    //validates the response
                    if (response <= 0 || response>options.Length)
                    {
                        throw new ArgumentException($"{response} is not an integer");
                    }

                    successful = true;
                }
                catch (Exception e)
                {
                    Console.Write("Oops, that's not an option! ");
                }
            }
            
            return response;
        }
    }

}
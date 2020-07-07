using System;

namespace Paycom_Seminar_2020
{
    static class UI
    {
        public static int getMenuResponse(String[] options)
        {
            Console.WriteLine("Please enter the number of the selection you would like to make.");
            int response = getOptionsResponse(options);
            Console.WriteLine($"You selected {options[response-1]}");
            
            return response;
            
        }

        public static int getMenuResponseWithBackOption(String[] options)
        {
            String[] newOptions = new String[options.Length+1];
            for (int i = 0; i<options.Length; i++)
            {
                newOptions[i] = options[i];
            }
            newOptions[newOptions.Length-1] = "Back";
            return getMenuResponse(newOptions);
        }

        public static String askQuestionFreeResponse(String question)
        {
            String response = null;
            Console.WriteLine(question);
            response = Console.ReadLine();

            return response;
        }

        public static int askQuestionMultipleChoice(String question, String[] choices)
        {
            Console.WriteLine(question);
            int response = getOptionsResponse(choices);
            return response;
        }

        public static String getUsername(String[] names)
        {
            String username = null;

            String[] options = new String[names.Length+1];
            options[0] = "Create New Profile";
            for (int i = 0; i<names.Length; i++)
            {
                options[i+1] = names[i];
            }

            int userAnswer = UI.askQuestionMultipleChoice("Please choose a profile or create a new one:", options);
            if(userAnswer == 1)
            {
                String question = "Please enter your desired username:";
                bool successful = false;
                while (!successful)
                {
                    
                    String name = askQuestionFreeResponse(question);
                    if (name.Contains("\\") || name.Contains(";"))
                    {
                        Console.Write("A username cannot have a '\\' or a ';' in it. ");
                        question = "Please try again:";
                    }
                    else
                    {
                        username = name;
                        successful = true;
                    }
                }
            }
            else
            {
                username = options[userAnswer-1];
            }

            return username;
        }

        private static int getOptionsResponse(String[] options)
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
                    Console.WriteLine("Oops, that's not an option! Try again!");
                }
            }
            
            return response;
        }
    }

}
using System;
using System.Net.Sockets;

namespace Paycom_Seminar_2020
{
    class UI
    {
        private Client _client = null;
        private TcpClient _tcpClient = null;

        public UI(TcpClient tcpClient)
        {
            _client = new Client(tcpClient);
            _tcpClient = tcpClient;
        }
        public void start()
        {
            String[] names = _client.requestUsernames();
            String serverMessage = selectProfile(names);
            
            bool successful = false;
            while (!successful)
            {
                String serverResponse = _client.sendServerMessage(serverMessage);
                if (serverResponse.Substring(0,2).Equals(ServerMessageDecoder.NAME_TAKEN))
                {
                    Console.Write("That username is already taken. ");
                    serverMessage = askForNewName("user");
                    _client.setUsername(serverMessage.Substring(2));
                }
                else if (serverResponse.Substring(0,2).Equals(ServerMessageDecoder.PROFILE_DELETED))
                {
                    Console.WriteLine("That profile has been delted while you were deciding on a profile.");
                    start();
                    successful = true;
                }
                else if (serverResponse.Substring(0,2).Equals(ServerMessageDecoder.NO_ACTION_REQUIRED))
                {
                    successful = true;
                }
            }
            showMainMenu();
        }

        public void showMainMenu()
        {
            String[]  options = {"View Subscription Messages [0 new messages]", "Manage Subscriptions", "Manage Topics", "Refresh Menu", "Quit"};
            int userChoice = getMenuResponse(options);
            if (userChoice == 3)
            {
                manageTopics();
            }
            else if (userChoice == 5)
            {
                _tcpClient.Close();
            }
            else
            {
                showMainMenu();
            }
        }

        public void manageTopics()
        {
            String[] options = {"Create Topic", "Manage Topic"};
            int userResponse = getMenuResponseWithBackOption(options);
            if(userResponse == options.Length+1)
            {
                showMainMenu();
            }
            else if (userResponse == 1)
            {
                createNewTopic();
            }
        }
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
            Console.WriteLine(question);
            int response = getOptionsResponse(choices);
            return response;
        }

        public String selectProfile(String[] names)
        {
            String serverMessage = "";

            String[] options = new String[names.Length+1];
            options[0] = "Create New Profile";
            for (int i = 0; i<names.Length; i++)
            {
                options[i+1] = names[i];
            }

            int userAnswer = askQuestionMultipleChoice("Please choose a profile or create a new one:", options);
            if(userAnswer == 1)
            {
                serverMessage = ClientMessageEncoder.CREATE_PROFILE+askForNewName("user");
            }
            else
            {
                serverMessage += ClientMessageEncoder.LOG_IN;
                serverMessage += options[userAnswer-1];
            }

            return serverMessage;
        }

        public String askForNewName(String nameType)
        {
            String serverMessage = "";

            String question = $"Please enter your desired {nameType}name:";
            bool successful = false;
            while (!successful)
            {
                String name = askQuestionFreeResponse(question);
                if (name.Contains("\\") || name.Contains(";"))
                {
                    Console.Write($"A {nameType} name cannot have a '\\' or a ';' in it. ");
                    question = "Please try again:";
                }
                else
                {
                    serverMessage += name;
                    successful = true;
                }
            }

            return serverMessage;
        }

        public void createNewTopic()
        {
            bool successful = false;
            while (!successful)
            {
                Console.Write("If you change your mind, type the word 'cancel' to abort. ");
                String topicName = askForNewName("topic ");
                if (topicName.Equals("cancel"))
                {
                    successful = true;
                }
                else
                {
                    String serverMessage = ClientMessageEncoder.CREATE_TOPIC+topicName;
                    String serverResponse = _client.sendServerMessage(serverMessage);
                    if (serverResponse.Substring(0,2).Equals(ServerMessageDecoder.NAME_TAKEN))
                    {
                        Console.Write("That name is already taken. ");
                    }
                    else
                    {
                        successful = true;
                    }
                }
                
            }
            manageTopics();
            

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
                    Console.WriteLine("Oops, that's not an option! Try again!");
                }
            }
            
            return response;
        }
    }

}
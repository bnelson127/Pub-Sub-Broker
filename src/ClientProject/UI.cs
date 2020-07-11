using System;
using System.Net.Sockets;

namespace Paycom_Seminar_2020
{
    class UI
    {
        private Client _client = null;
        private TcpClient _tcpClient = null;
        private Menu _mainMenu = new Menu("Main Menu", new String[] {"View Subscription Messages", "Manage Subscriptions", "Manage Topics", "Refresh Menu", "Quit"}, false);
        private Menu _manageTopics = new Menu("Manage Topics", new String[] {"Create Topic", "Manage Topic"}, true);
        private Menu _manageSubscriptions = new Menu("Manage Subscriptions", new String[] {"Subscribe to Topic", "Unsubscribe from Topic"}, true);
        private Menu _manageSingleTopic = new Menu("Topic Management", new String[] {"Publish Message", "Message History", "Topic Settings", "Delete Topic"}, true);
        public UI(TcpClient tcpClient, Client client)
        {
            _client = client;
            _tcpClient = tcpClient;
        }
        public void start()
        {
            String[] names = _client.requestUsernames();
            String userChoice = selectProfile(names);
            
            bool successful = false;
            while (!successful)
            {
                String status = checkUsername(userChoice);
                if (status.Equals("taken"))
                {
                    makeStatement("Sorry, but that username is already taken.");
                    userChoice = ClientMessageEncoder.CREATE_PROFILE+getValidatedString("Please enter your desired username:");
                }
                else if (status.Equals("deleted"))
                {
                    makeStatement("Sorry, but that profile seems to have now been deleted.");
                    names = _client.requestUsernames();
                    userChoice = selectProfile(names);
                }
                else if (status.Equals("successful"))
                {
                    makeStatement("You have successfully logged in. Welcome!");
                    _client.setUsername(userChoice.Substring(2));
                    successful = true;
                }
            }
            showMainMenu();
        }

        public String checkUsername(String userChoice)
        {
            String status = "";
            String serverResponse = _client.sendServerMessage(userChoice);
            String responseID = serverResponse.Substring(0,2);
            if (responseID.Equals(ServerMessageDecoder.NAME_TAKEN))
            {
                status = "taken";
            }
            else if (responseID.Equals(ServerMessageDecoder.PROFILE_DELETED))
            {
                status  = "deleted"; 
            }
            else if (responseID.Substring(0,2).Equals(ServerMessageDecoder.NO_ACTION_REQUIRED))
            {
                status = "successful";
            }

            return status;
        }

        public void showMainMenu()
        {
            String userChoice = _mainMenu.displayMenu();
            if (userChoice.Equals("Manage Subscriptions"))
            {
                manageSubscriptions();
            }
            else if (userChoice.Equals("Manage Topics"))
            {
                manageTopics();
            }
            else if (userChoice.Equals("Quit"))
            {
                _tcpClient.Close();
            }
        }

        public void manageTopics()
        {
            String userChoice = _manageTopics.displayMenu();
            if(userChoice.Equals("Back"))
            {
                showMainMenu();
            }
            else if (userChoice.Equals("Create Topic"))
            {
                createNewTopic();
            }
            else if (userChoice.Equals("Manage Topic"))
            {
                String[] topicOptions = _client.requestMyTopicNames();
                int userResponse = askQuestionMultipleChoice("Which topic would you like to manage?", topicOptions);
                String topicSelected = topicOptions[userResponse-1];
                makeStatement($"You have selelcted to manage the topic '{topicSelected}'");
                manageSingleTopic(topicSelected);
            }
        }

        public void manageSubscriptions()
        {
            String userChoice = _manageSubscriptions.displayMenu();
            if (userChoice.Equals("Back"))
            {
                showMainMenu();
            }
            else if (userChoice.Equals("Subscribe to Topic"))
            {
                subscribeToTopic();
            }
            else if (userChoice.Equals("Unsubscribe from Topic"))
            {

            }
        }

        public void manageSingleTopic(String topicName)
        {
            String userChoice = _manageSingleTopic.displayMenu();
            if (userChoice.Equals("Back"))
            {
                manageTopics();
            }
            else if (userChoice.Equals("Publish Message"))
            {
                publishMessage(topicName);
            }
        }

        public void publishMessage(String topicName)
        {
            String message = askQuestionFreeResponse("What would you like to publish to your subscribers?");
            _client.publishMessage(topicName, message);
            makeStatement("The message has been sent.");
            manageSingleTopic(topicName);

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
                serverMessage = ClientMessageEncoder.CREATE_PROFILE+getValidatedString("Please enter your desired username:");
            }
            else
            {
                serverMessage += ClientMessageEncoder.LOG_IN;
                serverMessage += options[userAnswer-1];
            }

            return serverMessage;
        }

        public void createNewTopic()
        {
            bool successful = false;
            String topicName = "";
            while (!successful)
            {
                Console.Write("If you change your mind, type the word 'cancel' to abort. ");
                topicName = getValidatedString("Please enter the desired name for your topic:");
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
                        makeStatement($"Sorry, but the name '{topicName}' is already taken.");
                    }
                    else
                    {
                        successful = true;
                    }
                }
                
            }
            makeStatement($"You have succesfully created the topic called '{topicName}'.");
            manageTopics();
            

        }

        public void subscribeToTopic()
        {
            String[] names = _client.requestNotYetSubscribedTopicNames();
            int totalNumberOfTopics = _client.requestAllTopicNames().Length;
            if (totalNumberOfTopics == 0)
            {
                makeStatement("Well this is embarrassing. There don't appear to be any topics to subscribe to.");
            }
            else if (names.Length == 0)
            {
                makeStatement("Wow! It looks like you've already subscribed to every single topic!");
            }
            else
            {
                int topicInt = askQuestionMultipleChoice("Select a topic to subscribe to: ", names);
                String topic = names[topicInt-1];
                Console.WriteLine(topic);
                _client.subscribeToTopic(topic);
                makeStatement($"Congratualtions! You have successfully subscribed to {topic}");
            }
            manageSubscriptions();
        }
        private int askQuestionMultipleChoice(String question, String[] options)
        {
            Console.WriteLine();
            Console.WriteLine(question);
            for (int i = 0; i<question.Length; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine("");

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

        public String askQuestionFreeResponse(String question)
        {
            String response = null;

            Console.WriteLine(question);
            for (int i = 0; i<question.Length; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine("");
            
            response = Console.ReadLine();

            return response;
        }
        public String getValidatedString(String question)
        {
            String serverMessage = "";

            bool successful = false;
            while (!successful)
            {
                String name = askQuestionFreeResponse(question);
                if (name.Contains("\\") || name.Contains(";"))
                {
                    Console.Write($"Sorry, but you cannot have a '\\' or a ';' in it. ");
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

        public void makeStatement(String statement)
        {
            Console.WriteLine("");
            Console.WriteLine(statement);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }

}
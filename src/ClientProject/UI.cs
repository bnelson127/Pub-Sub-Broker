using System;
using System.Net.Sockets;

namespace Paycom_Seminar_2020
{
    class UI
    {
        private Client _client = null;
        public UI(Client client)
        {
            _client = client;
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

        public void quit()
        {
            _client.closeConnections();
        }

        public String getTopicToManage()
        {
            String[] topicOptions = _client.requestMyTopicNames();
            int userResponse = askQuestionMultipleChoice("Which topic would you like to manage?", topicOptions);
            String topicSelected = topicOptions[userResponse-1];
            makeStatement($"You have selelcted to manage the topic '{topicSelected}'");
            return topicSelected;
        }
        public void publishMessage(String topicName)
        {
            String message = askQuestionFreeResponse("What would you like to publish to your subscribers?");
            _client.publishMessage(topicName, message);
            makeStatement("The message has been sent.");

        }

        public void viewSubscriptionMessages()
        {
            String[] mySubs = _client.requestSubscriptionNames();
            if (mySubs.Length == 0)
            {
                makeStatement("It doesn't look like you're subscribed to any topics! Go to the Manage Subscriptions menu to subscribe to some!");
            }
            else
            {
                int userChoice = askQuestionMultipleChoice("From what topic would you like to view messages?", mySubs);
                String topicName = mySubs[userChoice-1];
                String[] messages = _client.requestSubscriptionMessages(topicName);
                for (int i = messages.Length-1; i>=0; i-=2)
                {
                    Console.Write(messages[i-1]+": ");
                    Console.WriteLine(messages[i]);
                }
            }
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
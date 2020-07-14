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
                    makeStatement($"You have successfully logged in as {userChoice.Substring(2)}. Welcome!");
                    _client.setUsername(userChoice.Substring(2));
                    successful = true;
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
            if (topicOptions.Length > 0)
            {
                int userResponse = askQuestionMultipleChoice("Which topic would you like to manage?", topicOptions);
                String topicSelected = topicOptions[userResponse-1];
                makeStatement($"'{topicSelected}' is the topic that you have selelcted to manage.");
                return topicSelected;
            }
            else
            {
                makeStatement("It doesn't look like you own any topics to manage. Why don't you make one?");
                return null;
            }
            
        }
        public void viewTopicHistory(String topicName)
        {
            String[] messages = _client.requestTopicHistory(topicName);
            if (messages.Length > 1)
            {
                String title = "MESSAGE HISTORY OF "+topicName.ToUpper();

                String[] combinedMessages = new String[messages.Length/2];
                for (int i = 0; i<messages.Length; i+=2)
                {
                    combinedMessages[i/2] = (messages[i]+": "+messages[i+1]);
                }

                String postStatement = "These are all the messages that have been sent on this topic.";

                printListWithTitle(title, combinedMessages, postStatement);
            }
            else
            {
                makeStatement("It doesn't look like any messages have been sent on this topic. You should send some!");
            }
        }

        public void changeWelcomeMessage(String topicName)
        {
            String currentWelcomeMessage = _client.requestWelcomeMessage(topicName);
            String question = $"The current welcome message is '{currentWelcomeMessage}'. What would you like to change it to?";
            String newWelcomeMessage = askQuestionFreeResponse(question);
            _client.setWelcomeMessage(topicName, newWelcomeMessage);
            makeStatement($"The welcome message for '{topicName}' has been changed.");
        }

        public void publishMessage(String topicName)
        {
            String message = askQuestionFreeResponse("What would you like to publish to your subscribers?");
            _client.publishMessage(topicName, message);
            makeStatement("The message has been sent.");

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

        public void viewMyTopics()
        {
            String[] topics = _client.requestMyTopicNames();
            if (topics.Length>0)
            {
                String title = "MY TOPICS";
                String postStatement = "These are all the topics that you own.";
                printListWithTitle(title, topics, postStatement);
            }
            else
            {
                makeStatement("It doesn't look like you own any topics. You should create one!");
            }
            
        }

        public void toggleAutoRun(String topicName)
        {
            String[] defaultMessages = _client.requestDefaultMessages(topicName);
            if (defaultMessages.Length>0)
            {
                String state = _client.toggleAutoRun(topicName);
                if (state.Equals("true"))
                {
                    makeStatement("This topic has started automatically publishing messages.");
                }
                else
                {
                    makeStatement("This topic no longer automatically publishes messages.");
                }
            }
            else
            {
                makeStatement("Sorry, but it doesn't look like you have any default messages to automatically send. You can add some under the 'Manage Default Messages' menu.");
            }
        }

        public void addDefaultMessage(String topicName)
        {
            String message = askQuestionFreeResponse($"What message would you like to add to '{topicName}'?");
            _client.addDefaultMessage(topicName, message);
            makeStatement("The message has been added.");
        }

        public void deleteDefaultMessage(String topicName)
        {
            String[] messages = _client.requestDefaultMessages(topicName);
            if (messages.Length > 0)
            {
                int userChoice = askQuestionMultipleChoice("Which of these would you like to delete?", messages);
                String messageToDelete = messages[userChoice-1];
                int numMessagesLeft = _client.deleteDefaultMessage(topicName, messageToDelete);
                if (numMessagesLeft == 0)
                {
                    makeStatement("The message has been deleted. There are no messages left, so auto run has been disabled.");
                }
                else
                {
                    makeStatement("The message has been deleted.");
                }
            }
            else
            {
                makeStatement("It doesn't look like you have any default messages to delete!");
            }
        }

        public void viewDefaultMessages(String topicName)
        {
            String[] messages = _client.requestDefaultMessages(topicName);
            if (messages.Length > 0)
            {
                String title = $"DEFAULT MESSAGES FOR {topicName.ToUpper()}";
                String postStatement = $"These are all the messages that can be automatically sent for '{topicName}'.";
                printListWithTitle(title, messages, postStatement);
            }
            else
            {
                makeStatement("It doesn't look like you have any default messages for this topic!");
            }
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
                makeStatement($"From {topic}: "+_client.requestWelcomeMessage(topic));
            }
        }

        public void unsubscribeFromTopic()
        {
            String[] myTopics = _client.requestSubscriptionNames();
            String[] options = new String[myTopics.Length+1];
            options[0] = "Cancel";
            for (int i = 0; i<myTopics.Length; i++)
            {
                options[i+1] = myTopics[i];
            }
            int userResponse = askQuestionMultipleChoice("Select a topic to unsubscribe from or cancel.", options);
            if (userResponse == 1)
            {
                makeStatement("You have canceled the unsubscription.");
            }
            else
            {
                String topicName = options[userResponse-1];
                _client.unsubscribeFromTopic(topicName);
                makeStatement($"You have successfully unsubscribed from '{topicName}'");
            }
        }

        public void viewMySubscriptions()
        {
            String[] topics = _client.requestSubscriptionNames();
            if (topics.Length>0)
            {
                String title = "MY SUBSCRIPTIONS";
                String postStatement = "The are all the topics to which you are subscribed.";
                printListWithTitle(title, topics, postStatement);
            }
            else
            {
                makeStatement("It doesn't look like you're subscribed to any topics. You should go subscribe to some!");
            }
            
        }
        public void viewSubscriptionMessages()
        {
            String[] mySubs = _client.requestSubscriptionNames();
            int[] newMessageCounts = new int[mySubs.Length];
            for (int i = 0; i<mySubs.Length; i++)
            {
                newMessageCounts[i] = _client.requestNewMessageCount(mySubs[i]);
            }

            String[] menuOptions = new String[mySubs.Length];
            for (int i = 0; i<mySubs.Length; i++)
            {
                menuOptions[i] = $"[{newMessageCounts[i]} new] {mySubs[i]}";
            }

            if (mySubs.Length == 0)
            {
                makeStatement("It doesn't look like you're subscribed to any topics! Go to the Manage Subscriptions menu to subscribe to some!");
            }
            else
            {
                int userChoice = askQuestionMultipleChoice("From what topic would you like to view messages?", menuOptions);
                String topicName = mySubs[userChoice-1];

                String title = "MESSAGES FROM "+topicName.ToUpper();

                String[] messages = _client.requestSubscriptionMessages(topicName);
                String[] combinedMessages = new String[messages.Length/2];
                for (int i = 0; i<messages.Length; i+=2)
                {
                    combinedMessages[i/2] = (messages[i]+": "+messages[i+1]);
                }

                String postStatement = "These are all the messages you have recieved since joining the topic.";

                printListWithTitle(title, combinedMessages, postStatement);
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

        private String askQuestionFreeResponse(String question)
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
        private String getValidatedString(String question)
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

        private void makeStatement(String statement)
        {
            Console.WriteLine("");
            Console.WriteLine(statement);
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }

        private void printListWithTitle(String title, String[] list, String postStatement)
        {
            Console.WriteLine("");
            for (int i = 0; i<title.Length; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine("");
            Console.WriteLine(title);
            for (int i = 0; i<title.Length; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();

            foreach (String item in list)
            {
                Console.WriteLine(item);
            }

            makeStatement(postStatement);
        }
    }

}
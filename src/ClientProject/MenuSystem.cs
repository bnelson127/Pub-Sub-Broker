/*
This class is responsible for getting user responses from menus and then
deciding what to do based on that response.
*/

using System;

namespace Paycom_Seminar_2020
{
    class MenuSystem
    {
        private Menu _mainMenu = new Menu("Main Menu", new String[] {"View Subscription Messages", "Manage Subscriptions", "Manage Topics", "Quit"}, false);
        private Menu _manageTopics = new Menu("Manage Topics", new String[] {"View My Topics", "Manage Topic", "Create Topic"}, true);
        private Menu _manageSubscriptions = new Menu("Manage Subscriptions", new String[] {"View Subscriptions", "Subscribe to Topic", "Unsubscribe from Topic"}, true);
        private Menu _manageSingleTopic = new Menu("Topic Management", new String[] {"Publish Message", "Message History", "Topic Settings"}, true);
        private Menu _topicSettings = new Menu("Topic Settings", new String[] {"Toggle Auto Run", "Manage Default Messages", "Change Welcome Message"}, true);
        private Menu _manageDefaultMessages = new Menu("Default Message Management", new String[] {"View Default Messages", "Add Default Message", "Delete Default Message"}, true);
       
        private UI ui = null;

        public MenuSystem(UI Ui)
        {
            ui = Ui;
        }
        public void start()
        {
            ui.start();
            showMainMenu();
        }
        public void showMainMenu()
        {
            String userChoice = _mainMenu.displayMenu();
            if (userChoice.Equals("View Subscription Messages"))
            {
                ui.viewSubscriptionMessages();
                showMainMenu();
            }
            else if (userChoice.Equals("Manage Subscriptions"))
            {
                manageSubscriptions();
            }
            else if (userChoice.Equals("Manage Topics"))
            {
                manageTopics();
            }
            else if (userChoice.Equals("Quit"))
            {
                ui.quit();
            }
        }

        public void manageTopics()
        {
            String userChoice = _manageTopics.displayMenu();
            if(userChoice.Equals("Back"))
            {
                showMainMenu();
            }
            else if (userChoice.Equals("View My Topics"))
            {
                ui.viewMyTopics();
                manageTopics();
            }
            else if (userChoice.Equals("Create Topic"))
            {
                ui.createNewTopic();
                manageTopics();
            }
            else if (userChoice.Equals("Manage Topic"))
            {
                String topicSelected = ui.getTopicToManage();
                if (topicSelected == null)
                {
                    manageTopics();
                }
                else
                {
                    manageSingleTopic(topicSelected);
                }
                
            }
        }

        public void manageSubscriptions()
        {
            String userChoice = _manageSubscriptions.displayMenu();
            if (userChoice.Equals("Back"))
            {
                showMainMenu();
            }
            else if (userChoice.Equals("View Subscriptions"))
            {
                ui.viewMySubscriptions();
                manageSubscriptions();
            }
            else if (userChoice.Equals("Subscribe to Topic"))
            {
                ui.subscribeToTopic();
                manageSubscriptions();
            }
            else if (userChoice.Equals("Unsubscribe from Topic"))
            {
                ui.unsubscribeFromTopic();
                manageSubscriptions();
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
                ui.publishMessage(topicName);
                manageSingleTopic(topicName);
            }
            else if (userChoice.Equals("Message History"))
            {
                ui.viewTopicHistory(topicName);
                manageSingleTopic(topicName);
            }
            else if (userChoice.Equals("Topic Settings"))
            {
                manageTopicSettings(topicName);
            }
        }

        public void manageTopicSettings(String topicName)
        {
            String userChoice = _topicSettings.displayMenu();
            if (userChoice.Equals("Back"))
            {
                manageSingleTopic(topicName);
            }
            else if (userChoice.Equals("Toggle Auto Run"))
            {
                ui.toggleAutoRun(topicName);
                manageTopicSettings(topicName);
            }
            else if (userChoice.Equals("Manage Default Messages"))
            {
                manageDefaultMessages(topicName);
            }
            else if (userChoice.Equals("Change Welcome Message"))
            {
                ui.changeWelcomeMessage(topicName);
                manageTopicSettings(topicName);
            }
        }

        public void manageDefaultMessages(String topicName)
        {
            String userChoice = _manageDefaultMessages.displayMenu();
            if (userChoice.Equals("Back"))
            {
                manageTopicSettings(topicName);
            }
            else if (userChoice.Equals("View Default Messages"))
            {
                ui.viewDefaultMessages(topicName);
                manageDefaultMessages(topicName);
            }
            else if (userChoice.Equals("Add Default Message"))
            {
                ui.addDefaultMessage(topicName);
                manageDefaultMessages(topicName);
            }
            else if (userChoice.Equals("Delete Default Message"))
            {
                ui.deleteDefaultMessage(topicName);
                manageDefaultMessages(topicName);
            }
        }
    }

}
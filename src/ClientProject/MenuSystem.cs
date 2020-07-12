using System;

namespace Paycom_Seminar_2020
{
    class MenuSystem
    {
        private Menu _mainMenu = new Menu("Main Menu", new String[] {"View Subscription Messages", "Manage Subscriptions", "Manage Topics", "Refresh Menu", "Quit"}, false);
        private Menu _manageTopics = new Menu("Manage Topics", new String[] {"Create Topic", "Manage Topic"}, true);
        private Menu _manageSubscriptions = new Menu("Manage Subscriptions", new String[] {"Subscribe to Topic", "Unsubscribe from Topic"}, true);
        private Menu _manageSingleTopic = new Menu("Topic Management", new String[] {"Publish Message", "Message History", "Topic Settings", "Delete Topic"}, true);
       
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
            else if (userChoice.Equals("Create Topic"))
            {
                ui.createNewTopic();
                manageTopics();
            }
            else if (userChoice.Equals("Manage Topic"))
            {
                String topicSelected = ui.getTopicToManage();
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
                ui.subscribeToTopic();
                manageSubscriptions();
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
                ui.publishMessage(topicName);
                manageSingleTopic(topicName);
            }
        }
    }

}
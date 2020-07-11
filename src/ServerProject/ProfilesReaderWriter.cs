using System;
using System.Collections;
using System.Xml;

namespace Paycom_Seminar_2020
{
   
    class ProfilesReaderWriter
    {
        private String _profilesFilePath = "files/profiles.xml";

        public String[] getUsernames()
        {
            ArrayList usernamesList = new ArrayList();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_profilesFilePath);
            XmlNodeList userNodes = xmlDoc.SelectNodes("//profiles/profile");
            foreach(XmlNode userNode in userNodes)
            {
                String username = userNode.Attributes["username"].Value;
                usernamesList.Add(username);
                Console.WriteLine(username);
            }

            String[] usernamesArray = Array.ConvertAll(usernamesList.ToArray(), x => x.ToString());
            
            return usernamesArray;
        }

        public Profile createNewProfile(String username)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_profilesFilePath);
            XmlNode rootNode = xmlDoc.SelectSingleNode("profiles");

            XmlNode profileNode = xmlDoc.CreateElement("profile");

            XmlAttribute profileName = xmlDoc.CreateAttribute("username");
            profileName.Value = username;
            profileNode.Attributes.Append(profileName);

            XmlElement subscriptions = xmlDoc.CreateElement("subscriptions");
            profileNode.AppendChild(subscriptions);

            XmlElement topics = xmlDoc.CreateElement("topics");
            profileNode.AppendChild(topics);

            rootNode.AppendChild(profileNode);

            xmlDoc.Save(_profilesFilePath);

            Profile profile = new Profile(username, new String[0], new String[0]);

            return profile;
        }

        public Profile loadProfile(String username)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_profilesFilePath);

            XmlNode profileNode = getProfileNode(xmlDoc, username);

            XmlNode subscriptionsNode = profileNode.SelectSingleNode("//subscriptions");
            XmlNode topicsNode = profileNode.SelectSingleNode("//topics");

            Profile profile = new Profile(profileNode.Attributes["username"].Value, new String[0], new String[0]);
            return profile;
        }

        public void addTopic(String username, String topicName)
        {
            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_profilesFilePath);

           XmlNode profileNode = getProfileNode(xmlDoc, username);
            
            XmlNode topicsNode = null;
            XmlNodeList subNodes = profileNode.ChildNodes;
            foreach(XmlNode child in subNodes)
            {
                if (child.Name.Equals("topics"))
                {
                    topicsNode = child;
                    
                }
                Console.WriteLine("USERNAME "+child.Name);
            }
            
            //topicsNode = profileNode.SelectSingleNode("//topics");
            

            XmlElement topic = xmlDoc.CreateElement("topic");
            XmlAttribute newTopicName = xmlDoc.CreateAttribute("name");
            newTopicName.Value = topicName;
            topic.Attributes.Append(newTopicName);

            topicsNode.AppendChild(topic);

            xmlDoc.Save(_profilesFilePath);

        }

        public void addSubscription(String username, String topicName)
        {
            
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_profilesFilePath);
            
            XmlNode subscriptionsNode = getSubscriptionsNode(xmlDoc, username);

            XmlElement subscription = xmlDoc.CreateElement("subscription");
            XmlAttribute newSubscriptionName = xmlDoc.CreateAttribute("name");
            newSubscriptionName.Value = topicName;
            subscription.Attributes.Append(newSubscriptionName);

            subscriptionsNode.AppendChild(subscription);

            xmlDoc.Save(_profilesFilePath);

        }

        public ArrayList getSubscriptions(String username)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_profilesFilePath);

            XmlNode subscriptionsNode = getSubscriptionsNode(xmlDoc, username);
            ArrayList subNamesList = new ArrayList();
            XmlNodeList subscriptionNodes = subscriptionsNode.ChildNodes;
            foreach(XmlNode subscription in subscriptionNodes)
            {
                String subName = subscription.Attributes["name"].Value;
                subNamesList.Add(subName);
                Console.WriteLine(subName);
            }

            return subNamesList;
            
        }

        private XmlNode getProfileNode(XmlDocument xmlDoc, String username)
        {
            xmlDoc.Load(_profilesFilePath);

            XmlNode profileNode = null;
            XmlNodeList userNodes = xmlDoc.SelectNodes("//profiles/profile");
            foreach(XmlNode userNode in userNodes)
            {
                if (userNode.Attributes["username"].Value.Equals(username))
                {
                    profileNode = userNode;
                }
            }

            return profileNode;
        }

        private XmlNode getSubscriptionsNode(XmlDocument xmlDoc, String username)
        {

            XmlNode profileNode = getProfileNode(xmlDoc, username);
            
            XmlNode subscriptionsNode = null;
            XmlNodeList subNodes = profileNode.ChildNodes;
            foreach(XmlNode child in subNodes)
            {
                if (child.Name.Equals("subscriptions"))
                {
                    subscriptionsNode = child;
                }
            }

            return subscriptionsNode;
        }
    }

}
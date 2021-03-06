/*
This class is responsible for accessing and editing the profile data stored in
profiles.xml. The class inherits from XMLReaderWriter
*/

using System;
using System.Collections;
using System.Xml;

namespace Paycom_Seminar_2020
{
   
    class ProfilesReaderWriter : XMLReaderWriter
    {
        private String _profilesFilePath = "files/profiles.xml";
        private Object _profileLock = null;

        public ProfilesReaderWriter(Object profileLock)
        {
            _profileLock = profileLock;
        }

        public String[] getUsernames()
        {
            lock (_profileLock)
            {
                ArrayList usernamesList = new ArrayList();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_profilesFilePath);
                XmlNodeList userNodes = xmlDoc.SelectNodes("//profiles/profile");
                foreach(XmlNode userNode in userNodes)
                {
                    String username = userNode.Attributes["name"].Value;
                    usernamesList.Add(username);
                }

                String[] usernamesArray = Array.ConvertAll(usernamesList.ToArray(), x => x.ToString());
                
                return usernamesArray;
            }
        }

        public Profile createNewProfile(String username)
        {
            lock (_profileLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_profilesFilePath);
                XmlNode rootNode = xmlDoc.SelectSingleNode("profiles");

                XmlNode profileNode = xmlDoc.CreateElement("profile");

                XmlAttribute profileName = xmlDoc.CreateAttribute("name");
                profileName.Value = username;
                profileNode.Attributes.Append(profileName);

                XmlElement subscriptions = xmlDoc.CreateElement("subscriptions");
                profileNode.AppendChild(subscriptions);

                XmlElement topics = xmlDoc.CreateElement("topics");
                profileNode.AppendChild(topics);

                rootNode.AppendChild(profileNode);

                xmlDoc.Save(_profilesFilePath);

                Profile profile = new Profile(username);

                return profile;
            }
        }

        public Profile loadProfile(String username)
        {
            lock (_profileLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_profilesFilePath);

                XmlNode profileNode = getTopNode("profile", xmlDoc, username);

                Profile profile = new Profile(profileNode.Attributes["name"].Value);
                return profile;
            }
        }

        public void addTopic(String username, String topicName)
        {
            lock (_profileLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_profilesFilePath);

                XmlNode profileNode = getTopNode("profile", xmlDoc, username);
                XmlNode topicsNode = getSubNode(profileNode, xmlDoc, "topics");

                XmlElement topic = xmlDoc.CreateElement("topic");
                XmlAttribute newTopicName = xmlDoc.CreateAttribute("name");
                newTopicName.Value = topicName;
                topic.Attributes.Append(newTopicName);

                topicsNode.AppendChild(topic);

                xmlDoc.Save(_profilesFilePath);
            }

        }

        public void addSubscription(String username, String topicName)
        {
            lock (_profileLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_profilesFilePath);
                
                XmlNode profileNode = getTopNode("profile", xmlDoc, username);
                XmlNode subscriptionsNode = getSubNode(profileNode, xmlDoc, "subscriptions");

                XmlElement subscription = xmlDoc.CreateElement("subscription");

                XmlAttribute newSubscriptionName = xmlDoc.CreateAttribute("name");
                newSubscriptionName.Value = topicName;
                subscription.Attributes.Append(newSubscriptionName);

                XmlAttribute timeStamp = xmlDoc.CreateAttribute("timeJoined");
                timeStamp.Value = DateTime.Now.Ticks.ToString();
                subscription.Attributes.Append(timeStamp);

                XmlAttribute lastChecked = xmlDoc.CreateAttribute("lastChecked");
                lastChecked.Value = DateTime.Now.Ticks.ToString();
                subscription.Attributes.Append(lastChecked);

                subscriptionsNode.AppendChild(subscription);

                xmlDoc.Save(_profilesFilePath);
            }

        }

        public void removeSubscription(String username, String topicName)
        {
            lock (_profileLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_profilesFilePath);

                XmlNode profileNode = getTopNode("profile", xmlDoc, username);
                XmlNode subscriptionsNode = getSubNode(profileNode, xmlDoc, "subscriptions");
                XmlNode removedNode = getSubNode(subscriptionsNode, xmlDoc, "subscription", topicName);
                subscriptionsNode.RemoveChild(removedNode);

                xmlDoc.Save(_profilesFilePath);
            }

        }

        public ArrayList getSubscriptions(String username)
        {
            lock (_profileLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_profilesFilePath);

                XmlNode profileNode = getTopNode("profile", xmlDoc, username);
                XmlNode subscriptionsNode = getSubNode(profileNode, xmlDoc, "subscriptions");
                ArrayList subNamesList = new ArrayList();
                XmlNodeList subscriptionNodes = subscriptionsNode.ChildNodes;
                foreach(XmlNode subscription in subscriptionNodes)
                {
                    String subName = subscription.Attributes["name"].Value;
                    subNamesList.Add(subName);
                }

                return subNamesList;
            }
            
        }

        public String[] getTopics(String username)
        {
            lock (_profileLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_profilesFilePath);

                XmlNode profileNode = getTopNode("profile", xmlDoc, username);
                XmlNode topicsNode = getSubNode(profileNode, xmlDoc, "topics");
                ArrayList topNamesList = new ArrayList();
                XmlNodeList topicNodes = topicsNode.ChildNodes;
                foreach(XmlNode subscription in topicNodes)
                {
                    String subName = subscription.Attributes["name"].Value;
                    topNamesList.Add(subName);
                }

                return Array.ConvertAll(topNamesList.ToArray(), x => x.ToString());
            }
        }

        public long getSubscriptionDate(String username, String topicName)
        {
            lock (_profileLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_profilesFilePath);

                XmlNode profileNode = getTopNode("profile", xmlDoc, username);
                XmlNode subscriptionsNode = getSubNode(profileNode, xmlDoc, "subscriptions");
                XmlNode subscriptionNode = getSubNode(subscriptionsNode, xmlDoc, "subscription", topicName);

                long subscriptionDate = Convert.ToInt64(subscriptionNode.Attributes["timeJoined"].Value);
                return subscriptionDate;
            }

        }

        public void updateLastChecked(String username, String topicName)
        {
            lock (_profileLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_profilesFilePath);

                XmlNode profileNode = getTopNode("profile", xmlDoc, username);
                XmlNode subscriptionsNode = getSubNode(profileNode, xmlDoc, "subscriptions");
                XmlNode subscriptionNode = getSubNode(subscriptionsNode, xmlDoc, "subscription", topicName);

                subscriptionNode.Attributes["lastChecked"].Value = DateTime.Now.Ticks.ToString();

                xmlDoc.Save(_profilesFilePath);
            }
        }

        public long getLastChecked(String username, String topicName)
        {
            lock (_profileLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_profilesFilePath);

                XmlNode profileNode = getTopNode("profile", xmlDoc, username);
                XmlNode subscriptionsNode = getSubNode(profileNode, xmlDoc, "subscriptions");
                XmlNode subscriptionNode = getSubNode(subscriptionsNode, xmlDoc, "subscription", topicName);

                String stringLastChecked = subscriptionNode.Attributes["lastChecked"].Value;
                long longLastChecked = Convert.ToInt64(stringLastChecked);

                return longLastChecked;
            }
        }
    }

}
using System;
using System.Xml;
using System.Collections;

namespace Paycom_Seminar_2020
{
   
    class TopicReaderWriter : XMLReaderWriter 
    {
        private String _topicsFilePath = "files/topics.xml";
        public String[] getTopicNames()
        {
            ArrayList usernamesList = new ArrayList();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_topicsFilePath);
            XmlNodeList userNodes = xmlDoc.SelectNodes("//topics/topic");
            foreach(XmlNode userNode in userNodes)
            {
                String username = userNode.Attributes["name"].Value;
                usernamesList.Add(username);
                Console.WriteLine(username);
            }

            String[] usernamesArray = Array.ConvertAll(usernamesList.ToArray(), x => x.ToString());
            
            return usernamesArray;
        }

        public Topic createNewTopic(String name)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_topicsFilePath);
            XmlNode rootNode = xmlDoc.SelectSingleNode("topics");

            XmlNode topicNode = xmlDoc.CreateElement("topic");

            XmlAttribute topicName = xmlDoc.CreateAttribute("name");
            topicName.Value = name;
            topicNode.Attributes.Append(topicName);

            XmlElement settings = xmlDoc.CreateElement("settings");

            XmlAttribute welcomeMessage = xmlDoc.CreateAttribute("welcomMessage");
            welcomeMessage.Value = "Welcome to the topic!";
            settings.Attributes.Append(welcomeMessage);

            XmlAttribute defaultMessages = xmlDoc.CreateAttribute("defaultMessages");
            defaultMessages.Value = ";";
            settings.Attributes.Append(defaultMessages);

            XmlAttribute autoSend = xmlDoc.CreateAttribute("autoSend");
            autoSend.Value = "false";
            settings.Attributes.Append(autoSend);
            
            topicNode.AppendChild(settings);

            XmlElement sentMessages = xmlDoc.CreateElement("sentMessages");
            topicNode.AppendChild(sentMessages);

            rootNode.AppendChild(topicNode);

            xmlDoc.Save(_topicsFilePath);

            Topic topic = new Topic(welcomeMessage.Value, false, "");

            return topic;
        }

        public void publishMessage(String topicName, String message)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_topicsFilePath);

            XmlNode topicNode = getTopNode("topic", xmlDoc, topicName);
            XmlNode sentMessagesNode = getSubNode(topicNode, xmlDoc, "sentMessages");

            XmlElement sentMessage = xmlDoc.CreateElement("sentMessage");

            XmlAttribute content = xmlDoc.CreateAttribute("content");
            content.Value = message;
            sentMessage.Attributes.Append(content);

            XmlAttribute timeStamp = xmlDoc.CreateAttribute("timestamp");
            timeStamp.Value = DateTime.Now.Ticks.ToString();
            sentMessage.Attributes.Append(timeStamp);
            

            sentMessagesNode.AppendChild(sentMessage);

            xmlDoc.Save(_topicsFilePath);


        }
    }

}
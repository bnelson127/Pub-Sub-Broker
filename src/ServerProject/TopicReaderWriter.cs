/*
This class is responsible for accessing and editing the topic data stored in
topics.xml. The class inherits from XMLReaderWriter.
*/

using System;
using System.Xml;
using System.Collections;

namespace Paycom_Seminar_2020
{
   
    class TopicReaderWriter : XMLReaderWriter 
    {
        private String _topicsFilePath = "files/topics.xml";
        private Object _topicLock = null;

        public TopicReaderWriter(Object topicLock)
        {
            _topicLock = topicLock;
        }
        public String[] getTopicNames()
        {
            lock (_topicLock)
            {
                ArrayList usernamesList = new ArrayList();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_topicsFilePath);
                XmlNodeList userNodes = xmlDoc.SelectNodes("//topics/topic");
                foreach(XmlNode userNode in userNodes)
                {
                    String username = userNode.Attributes["name"].Value;
                    usernamesList.Add(username);
                }

                String[] usernamesArray = Array.ConvertAll(usernamesList.ToArray(), x => x.ToString());
                
                return usernamesArray;
            }
        }

        public void createNewTopic(String name)
        {
            lock (_topicLock)
            {
                // This works and looks good. I am wondering if rather than creating an xml structure like this,
                // you could just create a JSON object on the fly. Something to think about. Basically C# allows
                // you to convert objects into JSON.
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_topicsFilePath);
                XmlNode rootNode = xmlDoc.SelectSingleNode("topics");

                XmlNode topicNode = xmlDoc.CreateElement("topic");

                XmlAttribute topicName = xmlDoc.CreateAttribute("name");
                topicName.Value = name;
                topicNode.Attributes.Append(topicName);

                XmlElement settings = xmlDoc.CreateElement("settings");

                XmlAttribute welcomeMessage = xmlDoc.CreateAttribute("welcomeMessage");
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
            }
        }

        public void publishMessage(String topicName, String message)
        {
            lock (_topicLock)
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

        public String[] getTopicMessages(String topicName, long joinTime)
        {
            lock (_topicLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_topicsFilePath);

                XmlNode topicNode = getTopNode("topic", xmlDoc, topicName);
                XmlNode sentMessagesNode = getSubNode(topicNode, xmlDoc, "sentMessages");

                ArrayList messages = new ArrayList();
                DateTime dateJoined = new DateTime(joinTime);
                messages.Add(dateJoined.ToShortDateString()+" "+dateJoined.ToShortTimeString());
                messages.Add("You joined the topic.");
                foreach (XmlNode message in sentMessagesNode)
                {
                    long timestamp = Convert.ToInt64(message.Attributes["timestamp"].Value);
                    if (timestamp >= joinTime)
                    {
                        DateTime date = new DateTime(timestamp);
                        messages.Add(date.ToShortDateString()+" "+date.ToShortTimeString());
                        messages.Add(message.Attributes["content"].Value);
                    }
                }
                return Array.ConvertAll(messages.ToArray(), x => x.ToString());
            }
        }

        public long[] getMessageTimeStamps(String topicName)
        {
            lock (_topicLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_topicsFilePath);

                XmlNode topicNode = getTopNode("topic", xmlDoc, topicName);
                XmlNode sentMessagesNode = getSubNode(topicNode, xmlDoc, "sentMessages");

                ArrayList timestamps = new ArrayList();
                foreach (XmlNode message in sentMessagesNode)
                {
                    long timestamp = Convert.ToInt64(message.Attributes["timestamp"].Value);
                    timestamps.Add(timestamp);
                }

                return Array.ConvertAll(timestamps.ToArray(), x => Convert.ToInt64(x));
            }
        }

        public String getAutoRun(String topicName)
        {
            lock (_topicLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_topicsFilePath);

                XmlNode topicNode = getTopNode("topic", xmlDoc, topicName);
                XmlNode settingsNode = getSubNode(topicNode, xmlDoc, "settings");

                return settingsNode.Attributes["autoSend"].Value;
            }
        }

        public String getDefaultMessages(String topicName)
        {
            lock (_topicLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_topicsFilePath);

                XmlNode topicNode = getTopNode("topic", xmlDoc, topicName);
                XmlNode settingsNode = getSubNode(topicNode, xmlDoc, "settings");

                return settingsNode.Attributes["defaultMessages"].Value;
            }
        }

        public String toggleAutoRun(String topicName)
        {
            lock (_topicLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_topicsFilePath);

                XmlNode topicNode = getTopNode("topic", xmlDoc, topicName);
                XmlNode settingsNode = getSubNode(topicNode, xmlDoc, "settings");

                String state = settingsNode.Attributes["autoSend"].Value;
                if (state.Equals("false"))
                {
                    settingsNode.Attributes["autoSend"].Value = "true";
                }
                else
                {
                    settingsNode.Attributes["autoSend"].Value = "false";
                }

                xmlDoc.Save(_topicsFilePath);

                return settingsNode.Attributes["autoSend"].Value;
            }
        }

        public void addDefaultMessage(String topicName, String message)
        {
            lock (_topicLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_topicsFilePath);

                XmlNode topicNode = getTopNode("topic", xmlDoc, topicName);
                XmlNode settingsNode = getSubNode(topicNode, xmlDoc, "settings");

                String messages = settingsNode.Attributes["defaultMessages"].Value;
                settingsNode.Attributes["defaultMessages"].Value = messages+message+";";

                xmlDoc.Save(_topicsFilePath);
            }

        }
        public void setDefaultMessages(String topicName, String messages)
        {
            lock (_topicLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_topicsFilePath);

                XmlNode topicNode = getTopNode("topic", xmlDoc, topicName);
                XmlNode settingsNode = getSubNode(topicNode, xmlDoc, "settings");

                settingsNode.Attributes["defaultMessages"].Value = messages;

                xmlDoc.Save(_topicsFilePath);
            }

        }

        public String getWelcomeMessage(String topicName)
        {
            lock (_topicLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_topicsFilePath);

                XmlNode topicNode = getTopNode("topic", xmlDoc, topicName);
                XmlNode settingsNode = getSubNode(topicNode, xmlDoc, "settings");

                String welcomeMessage = settingsNode.Attributes["welcomeMessage"].Value;

                return welcomeMessage;
            }
        }

        public void setWelcomeMessage(String topicName, String welcomeMessage)
        {
            lock (_topicLock)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_topicsFilePath);

                XmlNode topicNode = getTopNode("topic", xmlDoc, topicName);
                XmlNode settingsNode = getSubNode(topicNode, xmlDoc, "settings");

                settingsNode.Attributes["welcomeMessage"].Value = welcomeMessage;

                xmlDoc.Save(_topicsFilePath);
            }
        }
    }

}
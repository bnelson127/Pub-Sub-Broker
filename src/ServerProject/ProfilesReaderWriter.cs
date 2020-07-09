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

            XmlNode profileNode = null;
            XmlNodeList userNodes = xmlDoc.SelectNodes("//profiles/profile");
            foreach(XmlNode userNode in userNodes)
            {
                if (userNode.Attributes["username"].Value.Equals(username))
                {
                    profileNode = userNode;
                }
            }
            XmlNode subscriptionsNode = profileNode.SelectSingleNode("//subscriptions");
            XmlNode topicsNode = profileNode.SelectSingleNode("//topics");

            Profile profile = new Profile(profileNode.Attributes["username"].Value, new String[0], new String[0]);
            return profile;
        }
    }

}
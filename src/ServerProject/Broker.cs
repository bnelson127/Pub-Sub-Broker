using System;
using System.Xml;
using System.Collections;

namespace Paycom_Seminar_2020
{
   
    class Broker
    {
        private String _profilesFilePath = "files/profiles.xml";
        public string getResponse(string message)
        {
            string response = "dummy";
            string indicator = message.Substring(0,2);
            message = message.Substring(2);
            message = message.Trim((char) 0);

            if (indicator.Equals(ClientMessageDecoder.REQUEST_USERNAMES))
            {
                String[] usernamesArray = getUsernames();
                response = prepareStringArray(usernamesArray);

            }
            else if (indicator.Equals(ClientMessageDecoder.LOG_IN))
            {
                String[] existingUsernames = getUsernames();
                if (Array.Find(existingUsernames, element => element.Equals(message))!=null)
                {
                    response = ServerMessageEncoder.NO_ACTION_REQUIRED+"Successfully logged in.";
                }
                else
                {
                    response = ServerMessageEncoder.PROFILE_DELETED;
                }
                
            }
            else if (indicator.Equals(ClientMessageDecoder.CREATE_PROFILE))
            {
                response = createProfile(message);
            }

            return response;
        }

        private String[] getUsernames()
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

        private String createProfile(String username)
        {
            String responseMessage = "";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_profilesFilePath);

            String[] existingUsernames = getUsernames();
            //makes absolutely sure that the username has not already been taken.
            if (Array.Find(existingUsernames, element => element.Equals(username))==null)
            {
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
                responseMessage = ServerMessageEncoder.NO_ACTION_REQUIRED+"Profile successfully created.";
            }
            else
            {
                responseMessage = ServerMessageEncoder.NAME_TAKEN+"Sorry, that username was taken while you were deciding.";
            }

            return responseMessage;

        }

        private String prepareStringArray(String[] array)
        {
            String finished = "";
            for (int i = 0; i<array.Length; i++)
            {
                finished+=array[i]+";";
            }
            return finished;
        }
    }

}
using System;
using System.Xml;
using System.Collections;

namespace Paycom_Seminar_2020
{
   
    abstract class XMLReaderWriter
    {
        public XmlNode getTopNode(String nodeType, XmlDocument xmlDoc, String nodeName)
        {
            XmlNode profileNode = null;
            XmlNodeList userNodes = xmlDoc.SelectNodes($"//{nodeType}s/{nodeType}");
            foreach(XmlNode userNode in userNodes)
            {
                if (userNode.Attributes["name"].Value.Equals(nodeName))
                {
                    profileNode = userNode;
                }
            }

            return profileNode;
        }

        public XmlNode getSubNode(XmlNode parent, XmlDocument xmlDoc, String nodeName)
        {
            XmlNode topicsNode = null;
            XmlNodeList subNodes = parent.ChildNodes;
            foreach(XmlNode child in subNodes)
            {
                if (child.Name.Equals(nodeName))
                {
                    topicsNode = child;
                }
            }

            return topicsNode;
        }
    }

}
using System;
using System.Xml;
using System.Collections;

namespace Paycom_Seminar_2020
{
   
    abstract class XMLReaderWriter
    {
        public XmlNode getTopNode(String nodeType, XmlDocument xmlDoc, String nodeName)
        {
            XmlNode topNode = null;
            XmlNodeList subNodes = xmlDoc.SelectNodes($"//{nodeType}s/{nodeType}");
            foreach(XmlNode subNode in subNodes)
            {
                if (subNode.Attributes["name"].Value.Equals(nodeName))
                {
                    topNode = subNode;
                }
            }

            return topNode;
        }

        public XmlNode getSubNode(XmlNode parent, XmlDocument xmlDoc, String nodeType)
        {
            XmlNode subNode = null;
            XmlNodeList subNodes = parent.ChildNodes;
            foreach(XmlNode child in subNodes)
            {
                if (child.Name.Equals(nodeType))
                {
                    subNode = child;
                }
            }

            return subNode;
        }

        public XmlNode getSubNode(XmlNode parent, XmlDocument xmlDoc, String nodeType, String nodeName)
        {
            XmlNode subNode = null;
            XmlNodeList subNodes = parent.ChildNodes;
            foreach(XmlNode child in subNodes)
            {
                if (child.Attributes["name"].Value.Equals(nodeName))
                {
                    subNode = child;
                }
            }

            return subNode;
        }
    }

}
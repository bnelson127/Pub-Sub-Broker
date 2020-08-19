/*
This class is the parent class for ProfilesReaderWriter and TopicReaderWriter. It
contains methods that are useful to both.
*/

using System;
using System.Xml;

namespace Paycom_Seminar_2020
{
    // This is a good, clean class. 
    abstract class XMLReaderWriter
    {
        /*
        This method gets an xml node that is called whatever is in the parameter
        'nodeType' and that also has an attribute called 'name' whose value is
        equal to the parameter 'nodeName'
        */
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

        /*
        This method gets an xml node that is contained within the given parent node
        that is called whatever value is in the parameter 'nodeType' This is used
        whenever there is only a single instance of that node type under the parent
        node
        */
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

        /*
        This method gets an xml node that is contained in the parent node that
        is of the type given in the parameter 'nodeType' and has the value of
        the parameter 'nodeName' in its attribute called 'name'. This is used
        when more than one of the same nodeType is contained under the parent node
        */
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
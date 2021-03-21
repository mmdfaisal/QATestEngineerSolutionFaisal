using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Utilities
{
    public static class Helper
    {
        //Helper method for loading the xml file
        public static XmlDocument LoadXml(string xmlFileName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFileName);
            return xmlDoc;
        }

        //In theory, the following three methods can be combined in one method, which takes fileName and xPath
        //But I think this is better, because the calling method does not have to specify a string(fileName), which can lead to mistakes, typos etc
        //Also this makes it for easier readability
        //Also there are only 3 config files/methods
        public static string LoadFromContrastConfig(string xPath)
        {
            return LoadXml(@".\ConfigFiles\ContrastConfig.xml").SelectSingleNode(xPath).InnerText;
        }
        public static string LoadFromResourceUrls(string xPath)
        {
            return LoadXml(@".\ConfigFiles\ResourceUrls.xml").SelectSingleNode(xPath).InnerText;
        }

        public static string LoadFromRetryConfig(string xPath)
        {
            return LoadXml(@".\ConfigFiles\RetryConfig.xml").SelectSingleNode(xPath).InnerText;
        }

        public static string EncodeToBase64(string plainText)
        {
            if (String.IsNullOrEmpty(plainText))
            {
                throw new ArgumentOutOfRangeException("The supplied text is either null or empty");
            }
            byte[] byteArray= Encoding.ASCII.GetBytes(plainText);
            return Convert.ToBase64String(byteArray);
        }

        public static string GetAuthorizationString(string apiKey)
        {
            return EncodeToBase64(apiKey);
        }

        public static T DeserializeJsonFiletoObject<T>(string fileName)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
        }

        //Returns true if listToFind is a subset of listToSearch. Otherwise, returns false
        public static bool IsListSubsetOfAnotherList(List<string> listToFind, List<string> listToSearch)
        {
            bool isFound = true;
            foreach (var item in listToFind)
            {
                isFound = listToSearch.Contains(item);
                if (isFound == false)
                {
                    return isFound;
                }
            }
            return isFound;
        }
    }
}

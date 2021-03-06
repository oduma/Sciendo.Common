﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Sciendo.Common.Serialization
{
    public static class Serializer
    {
        public static T Deserialize<T>(string xmlString,Func<string, bool> preSerializationCheck=null,Func<string, string> preSerializationProcessing=null ) where T : class
        {
            if (preSerializationCheck != null)
                if (!preSerializationCheck(xmlString))
                    throw new PreSerializationCheckException();
            if (preSerializationProcessing != null)
                xmlString = preSerializationProcessing(xmlString);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(new UTF8Encoding().GetBytes(xmlString));
            return xmlSerializer.Deserialize(ms) as T;
        }

        public static string Serialize<T>(T data) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            xmlSerializer.Serialize(ms, data);

            return new UTF8Encoding().GetString(ms.GetBuffer());
        }

        public static List<T> DeserializeFromFile<T>(string fileName) where T : class
        {
            if (!File.Exists(fileName))
                return new List<T>();
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<T>));
                return (xmlSerializer.Deserialize(fs) as List<T>);
            }
        }

        public static void SerializeToFile<T>(List<T> data, string fileName) where T : class
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<T>));
                xmlSerializer.Serialize(fs, data);
            }

        }
        public static void SerializeOneToFile<T>(T data, string fileName) where T : class
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(fs, data);
            }
        }

        public static T DeserializeOneFromFile<T>(string fileName,
            Func<string, bool> preSerializationCheck = null,
            Func<string, string> preSerializationProcessing = null) where T : class
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName);
            using (TextReader fs = File.OpenText(fileName))
            {
                var str = fs.ReadToEnd();
                return Deserialize<T>(str,preSerializationCheck,preSerializationProcessing);
            }
        }
    }

    public class PreSerializationCheckException : Exception
    {
    }
}

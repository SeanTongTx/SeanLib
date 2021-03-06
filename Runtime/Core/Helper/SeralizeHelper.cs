using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SeanLib.Core
{
    using System.Xml.Serialization;

    public static class SeralizeHelper 
    {
        /// <summary>
        /// 序列化byte数组
        /// </summary>
        /// <param name="obj">Object.</param>
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null)
                return new byte[] { };
            BinaryFormatter serializer = new BinaryFormatter();
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            serializer.Serialize(memStream, obj);
            return memStream.ToArray();
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns>The byte array.</returns>
        /// <param name="barray">Barray.</param>
        public static object FromByteArray(this byte[] barray)
        {
            if (barray.Length == 0)
                return null;
            System.IO.MemoryStream memStream = new System.IO.MemoryStream(barray);
            BinaryFormatter deserializer = new BinaryFormatter();
            object newobj=null;
            newobj = deserializer.Deserialize(memStream);
            memStream.Close();
            return newobj;
        }
        /// <summary>
        /// 序列化后base64
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToByteString(this object obj)
        {
            byte[] bytes = obj.ToByteArray();
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// base64字符串反序列化
        /// </summary>
        /// <param name="bytestring"></param>
        /// <returns></returns>
        public static T FromByteString<T>(this string bytestring) where T:class 
        {
            byte[] bytes= Convert.FromBase64String(bytestring);
            return bytes.FromByteArray() as T;
        }
        /// <summary>
        /// 序列化为Xml（未验证)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXml(this object obj)
        {
            Encoding encoding = Encoding.UTF8;
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            System.IO.TextWriter writer = new System.IO.StreamWriter(memStream, encoding);
            System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
            serializer.Serialize(memStream, obj);
            memStream.Position = 0;
            byte[] buf = new byte[memStream.Length];
            memStream.Read(buf, 0, buf.Length);
            return encoding.GetString(buf);
        }
        /// <summary>
        /// 由Xml反序列化(未验证)
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object FromXml(this string xml, Type type)
        {
            System.IO.StringReader memStream = new System.IO.StringReader(xml);
            XmlSerializer deserializer = new XmlSerializer(type);
            object newobj = deserializer.Deserialize(memStream);
            memStream.Close();
            return newobj;

        }
    }
}
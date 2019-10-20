
using System.IO;
using System.Text;
using UnityEngine;
namespace SeanLib.Core
{
    /// <summary>
    /// Extension methods for common functions
    /// </summary>
    public static class UnityExtensions
    {
        #region ToV2String

        /// <summary>
        /// Converts a Vector3 to a string in X, Y, Z format
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static string ToV2String(this Vector2 v2)
        {
            return string.Format("{0}, {1}", v2.x, v2.y);
        }

        // ToV3String

        #endregion

        #region ToV3String

        /// <summary>
        /// Converts a Vector3 to a string in X, Y, Z format
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static string ToV3String(this Vector3 v3)
        {
            return string.Format("{0}, {1}, {2}", v3.x, v3.y, v3.z);
        }

        // ToV3String

        #endregion

        #region UnityStringToBytes

        /// <summary>
        /// Converts a string to bytes, in a Unity friendly way
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte[] UnityStringToBytes(this string source)
        {
            // exit if null
            if (string.IsNullOrEmpty(source))
                return null;

            // convert to bytes
            using (MemoryStream compMemStream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(compMemStream, Encoding.UTF8))
                {
                    writer.Write(source);
                    writer.Close();

                    return compMemStream.ToArray();
                }
            }
        }

        // UnityStringToBytes

        #endregion

        #region UnityBytesToString

        /// <summary>
        /// Converts a byte array to a Unicode string, in a Unity friendly way
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string UnityBytesToString(this byte[] source)
        {
            // exit if null
            if (source.IsNullOrEmpty())
                return string.Empty;

            // read from bytes
            using (MemoryStream compMemStream = new MemoryStream(source))
            {
                using (StreamReader reader = new StreamReader(compMemStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        // UnityBytesToString

        #endregion
    }
}
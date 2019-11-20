using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SeanLib.Core
{
    /// <summary>
    /// 文件工具 win下 读写文件 
    /// </summary>
    public class FileUtility
    {
        /// <summary>
        /// 创建并写入一个文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="info">文件信息</param>
        public static void CreateAndWriteFile(string path, string info)
        {
            FileStream file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            StreamWriter writer = new StreamWriter(file);
            writer.Write(info);
            writer.Close();
        }
        /// <summary>
        /// 读取一个文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件内容</returns>
        public static string ReadFile(string path)
        {
            StringBuilder info = new StringBuilder();
            try
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("GB2312")))
                {
                    string line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        info.Append(line).Append("\r\n");
                    }
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return info.ToString();
        }
    }
}

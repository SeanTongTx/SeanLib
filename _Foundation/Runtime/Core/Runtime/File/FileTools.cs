

namespace SeanLib.Core
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The file tools.
    /// </summary>
    public static class FileTools
    {
        public static DirectoryInfo VerifyDirection(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                var dirinfo=  Directory.CreateDirectory(dir);
                return dirinfo;
            }
            return null;
        }

        public static void Copy(string fromDir, string toDir, bool overwrite)
        {
            VerifyDirection(toDir);
            File.Copy(fromDir,toDir,overwrite);
        }

        public static object Create(string dir)
        {
            VerifyDirection(dir);
           return File.Create(dir);
        }

        public static void Delete(string dir)
        {
           File.Delete(dir);
        }

        public static bool Exists(string dir)
        {
           return File.Exists(dir);
        }

        public static void Move(string fromdir, string todir)
        {
            VerifyDirection(todir);
           File.Move(fromdir,todir);
        }

        public static FileStream Open(string dir, FileMode mode)
        {
           return File.Open(dir, mode);
        }
        public static byte[] ReadAllBytes(string dir)
        {
            FileInfo fi = new FileInfo(dir);
            long len = fi.Length;
            FileStream fs = new FileStream(dir, FileMode.Open);
            byte[] buffer = new byte[len];
            fs.Read(buffer, 0, (int)len);
            fs.Close();
            return buffer;
        }
        public static String ReadAllText(string dir)
        {
            if (!Exists(dir))
            {
                FileStream fs = Create(dir) as FileStream;
                fs.Close();
            }
            return File.ReadAllText(dir);
        }
        public static void WriteAllBytes(string dir,byte[] bs)
        {
            VerifyDirection(dir);
            File.WriteAllBytes(dir, bs);
        }
        public static void WriteAllText(string dir, string datas)
        {
            VerifyDirection(dir);
           File.WriteAllText(dir,datas,Encoding.UTF8);
        }

        /// 删除文件夹以及文件
        /// </summary>
        /// <param name="directoryPath"> 文件夹路径 </param>
        /// <param name="fileName"> 文件名称 </param>
        public static void DeleteDirectory(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            //删除文件
            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }

            //删除文件夹
            for (int i = 0; i < Directory.GetDirectories(directoryPath).Length; i++)
            {
                Directory.Delete(Directory.GetDirectories(directoryPath)[i], true);
            }
        }
    }
}

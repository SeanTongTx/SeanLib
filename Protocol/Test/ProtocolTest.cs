using System;
using System.Runtime.InteropServices;
using ProtocolCore.Serialize;
namespace Test
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using Protocol;
    using Protocol.Package;

    using ProtocolCore;
    using ProtocolCore.Compression;
    using ProtocolCore.Extend;
    using ProtocolCore.Protocol;

    using ServiceTools.Seralize;
    
    public class ProtocolTest
    {
        enum testEnum
        {
            a = 1,
        }

        [Test]
        public void TestSeralizePerformance()
        {
            ExtendProtocolSerialize.Enable();
            Class1 class1 = new Class1();
            class1.Class2.s = "classsedwqdsavdddddddddddddddddsdasdasddddddddddddddddd"
                              + "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd"
                              + "sdasddddddddddddddddd" + "asddddddddddddddd"
                              + "dasddddddddddddddddddddddxcvxcvxcvxcv"
                              + "cxvcxxxxxxxxxxxxxxxxxxxxxxxxczxcsadasdasdasdssssss";
            class1.b = true;
            class1.i = 12312;

            int TestCont = 100;

            for (int i = 0; i < 1000; i++)
            {
                class1.listValue.Add((byte)i);
            }
            class1.ArrayValue = class1.listValue.ToArray();
            /*     class1.Dictionary[1] = "dsasdasd";
                 class1.Dictionary[2] = "ddscccxz";*/
            class1.s = "192.168.1.37";
            #region 序列化反序列化性能

            Stopwatch watch = new Stopwatch();
            watch.Start();
            byte[] bytes = null;
            for (int i = 0; i < TestCont; i++)
            {
                bytes = ProtocolSerialize.Seralize(class1);
            }
            watch.Stop();
            UnityEngine.Debug.Log("seralizeTime:" + watch.ElapsedMilliseconds);
            string s = ProtocolSerialize.DebugSeralizeStr(bytes);
            watch.Restart();
            for (int i = 0; i < TestCont; i++)
            {
                Class1 class11 = ProtocolSerialize.Deseralize(typeof(Class1), bytes) as Class1;
            }
            watch.Stop();
            UnityEngine.Debug.Log("DeseralizeTime:" + watch.ElapsedMilliseconds);

           /* watch.Restart();
            for (int i = 0; i < TestCont; i++)
            {
                class1.ToByteArray();//2进制流输出
            }
            watch.Stop();
            UnityEngine.Debug.Log("binaryBytesTime:" + watch.ElapsedMilliseconds);
            */
            watch.Restart();
            for (int i = 0; i < TestCont; i++)
            {
                UnityEngine.JsonUtility.ToJson(class1);
            }
            watch.Stop();
            UnityEngine.Debug.Log("seralize json:" + watch.ElapsedMilliseconds);
            s = UnityEngine.JsonUtility.ToJson(class1);
            watch.Restart();
            for (int i = 0; i < TestCont; i++)
            {
                UnityEngine.JsonUtility.FromJson<Class1>(s);
            }
            watch.Stop();
            UnityEngine.Debug.Log("DeseralizeTime json:" + watch.ElapsedMilliseconds);
            #endregion

            #region 大小

            byte[] binaryBytes = class1.ToByteArray();//2进制流输出
            UnityEngine.Debug.Log("二进制流直接输出:" + binaryBytes.Length);
            byte[] CompBinartBytes = CompressBase.CompressBytes(binaryBytes);
            UnityEngine.Debug.Log("压缩后:" + CompBinartBytes.Length);

            bytes = ProtocolSerialize.Seralize(class1);
            UnityEngine.Debug.Log("Protocol序列化:" + bytes.Length);
            byte[] CompBbytes = CompressBase.CompressBytes(bytes);
            UnityEngine.Debug.Log("压缩后:" + CompBbytes.Length);



            string base64 = Convert.ToBase64String(bytes);
            UnityEngine.Debug.Log("base64:" + base64.Length);

            string str = UnityEngine.JsonUtility.ToJson(class1);// json.Serialize(class1);
            byte[] jsonbytes = Encoding.UTF8.GetBytes(str);
            UnityEngine.Debug.Log("Json序列化:" + jsonbytes.Length);
            byte[] Compjsonbytes = CompressBase.CompressBytes(jsonbytes);
            UnityEngine.Debug.Log("压缩后:" + Compjsonbytes.Length);
            jsonbytes = CompressBase.DecompressBytes(Compjsonbytes);
            str = Encoding.UTF8.GetString(jsonbytes);
            int a = 0;

            #endregion
        }

        [Test]
        public void TestPackage()
        {
            ExtendProtocolSerialize.Enable();
            RPCPackage package = new RPCPackage { Count = 1, Method = "sdsad", GUID = "sdasjhnckjxzc0" };
            byte[] bytes = ProtocolSerialize.Seralize(package);
            RPCPackage pack1 = ProtocolSerialize.Deseralize<RPCPackage>(bytes);
            Assert.AreEqual(package.Count, pack1.Count);
            Assert.AreEqual(package.Method, pack1.Method);
            Assert.AreEqual(package.GUID, pack1.GUID);
        }

        [Test]
        public void TestErrorData()
        {
            ExtendProtocolSerialize.Enable();
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
            ProtocolSerialize.compatibility = ProtocolSerialize.Compatibility.Strict;

            RPCPackage pack1 = ProtocolSerialize.Deseralize<RPCPackage>(bytes);
            Assert.IsTrue(pack1 == null || !pack1.Validity());
            ProtocolSerialize.compatibility = ProtocolSerialize.Compatibility.MostCompatible;

            RPCPackage pack11 = ProtocolSerialize.Deseralize<RPCPackage>(bytes);
            Assert.IsNotNull(pack11);
        }
        [Test]
        public void TestSeralize()
        {
            ExtendProtocolSerialize.Enable();
            Class1 class1 = new Class1();
            class1.ArrayValue = new int[] { 1, 2, 3, 4, 5 };
            class1.Class2.s = "classsssssss";
            // class1.Dictionary[1] = class1;
            class1.b = true;
            class1.i = 12312;
            class1.f = 5783f;
            class1.d = 231f;
            class1.Dictionary[1] = "dsasdasd";
            class1.Dictionary[2] = "ddscccxz";
            // class1.listObject.Add(class1);
            class1.listValue.Add(132);
            // class1.obj = class1;
            class1.s = "class111111";
            
            byte[] bytes = ProtocolSerialize.Seralize(class1);
            Class1 class11 = ProtocolSerialize.Deseralize(typeof(Class1), bytes) as Class1;
            Assert.IsTrue(class1.Equals(class11));
        }
        /// <summary>
        /// 扩展模式
        /// </summary>
        [Test]
        public void TestSerializeExtend()
        {
          /*  Class3 c3 = new Class3() { i = 2, s = "shjkcjx", class4 = new Class4() { i = 1, b = true } };
            byte[] bytes = null;
            Class3 c31 = null;
            try
            {
                bytes = ProtocolSerialize.Seralize(c3);
                c31 = ProtocolSerialize.Deseralize(typeof(Class3), bytes) as Class3;

                Assert.Fail("不应该能正常执行");
            }
            catch (Exception)
            {
                ExtendObjectSerialize.Enable();
            }

            bytes = ProtocolSerialize.Seralize(c3);
            c31 = ProtocolSerialize.Deseralize(typeof(Class3), bytes) as Class3;
            Assert.IsTrue(c3.Equals(c31));
            ExtendObjectSerialize.Unable();*/
        }

        [Test]
        public void TestHeadCode()
        {
            int index = 127;
            int length = 65535;
            int Head = ProtocolModule.EncodeHead(index, length);
            int DeIndex;
            int DeLength = ProtocolModule.DecodeHead(Head, out DeIndex);
            Assert.AreEqual(index, DeIndex);
            Assert.AreEqual(length, DeLength);
        }
        /// <summary>
        /// 检查值类型序列化反序列化
        /// </summary>
        [Test]
        public void TestValueSeralize()
        {
            int i = 112312;
            this.TestValueSeralize(i);

            bool b = true;
            this.TestValueSeralize(b);

            float f = 1.21543f;
            this.TestValueSeralize(f);
            double d = 2734.231d;
            this.TestValueSeralize(d);

            long l = 5131851434123123141L;
            this.TestValueSeralize(l);

            string s = "192.168.1.37";
            this.TestStringSeralize(s);

            testEnum e = testEnum.a;
            this.TestValueSeralize(e);

            DateTime date = new DateTime(1935, 1, 1, 15, 24, 6, 500);
            this.TestValueSeralize(date);
        }

        private void TestStringSeralize(string value)
        {
            SystemSerializer systemSerializer = new SystemSerializer();
            byte[] bs = systemSerializer.stringBytes(value);
            object obj = systemSerializer.BytesString(bs);
            string newvalue = obj as string;
            Assert.AreEqual(value, newvalue);
        }

        private void TestValueSeralize(ValueType value)
        {
            byte[] bs = SerializeValueType.valueBytes(value);
            Debug.WriteLine(value + "序列化后字节数" + bs.Length);
            object obj = DeserializeValueType.BytesValue(value.GetType(), bs);
            ValueType newvalue = obj as ValueType;
            Assert.AreEqual(value, newvalue);
        }

        /// <summary>
        /// 测试数组类型序列化和反序列化
        /// </summary>
        [Test]
        public void TestArraySeralize()
        {
            object[] emptyArray = new object[] { };
            TestArray(emptyArray);
            int[] intarray = new int[] { 123, 123, 1, 3, 3, 2, 1, 2 };
            TestArray(intarray);
            bool[] boolarray = new bool[] { true, false, true, true };
            TestArray(boolarray);
            float[] farray = new float[] { 12.58f, 12.1f, 123, 2f };
            TestArray(farray);
            string[] sarray = new string[] { "sdsddd", "vxcxcxc", "2333123", "231" };
            TestArray(sarray);
            long[] larray = new long[] { 1211111121234541, 4511154546542313584, 412341584875132 };
            TestArray(larray);
        }

        private void TestArray(Array array)
        {
            SystemSerializer systemSerializer = new SystemSerializer();
            byte[] bs = systemSerializer.valueBytes_Array(array);
            object obj = systemSerializer.BytesValue_Array(array.GetType(), bs);
            Array newarray = obj as Array;
            Assert.AreEqual(array.Length, newarray.Length);
            for (int i = 0; i < array.Length; i++)
            {
                Assert.AreEqual(array.GetValue(i), newarray.GetValue(i));
            }
        }
        [Test]
        public void TestList()
        {
            List<string> slist = new List<string>() { "12312341", "dxfsada", "cvv12131", "cvxzcsjkd1" };
            TestList(slist);
        }

        private void TestList(IList list)
        {
            SystemSerializer systemSerializer = new SystemSerializer();
            byte[] bs = systemSerializer.valueBytes_List(list);
            IList newlist = systemSerializer.Bytesvalue_IList(list.GetType(), bs) as IList;
            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(list[i], newlist[i]);
            }
        }

        [Test]
        public void TestDictionary()
        {
            Dictionary<int, string> intstringDic = new Dictionary<int, string>();
            intstringDic[1] = "djfknvkl";
            intstringDic[2] = "ggd";
            intstringDic[3] = "321";
            intstringDic[15] = "asaa";
            TestDictionary(intstringDic);
        }

        private void TestDictionary(IDictionary dic)
        {
            SystemSerializer systemSerializer = new SystemSerializer();
            byte[] bs = systemSerializer.valueBytes_Dictionary(dic);
            Dictionary<int, string> obj = systemSerializer.BytesValue_Dic(dic.GetType(), bs) as Dictionary<int, string>;
            Assert.AreEqual(obj.Count, dic.Count);
            foreach (KeyValuePair<int, string> pair in obj)
            {
                Assert.AreEqual(dic[pair.Key], pair.Value);
            }
        }

        [Test]
        public void TestBinaryStream()
        {
            using (MemoryStream streamBuffer = new MemoryStream())
            {
                using (BinaryWriter Writer = new BinaryWriter(streamBuffer))
                {
                    Writer.Write(12355512);
                    byte[] b = new byte[streamBuffer.Length];
                    streamBuffer.Seek(0, SeekOrigin.Begin);
                    streamBuffer.Read(b, 0, (int)streamBuffer.Length);
                }
            }
        }
    }
}

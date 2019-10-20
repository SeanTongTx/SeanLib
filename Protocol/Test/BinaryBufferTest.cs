
using NUnit.Framework;
using Protocol;
using ProtocolCore;
using ProtocolCore.Compression;
using ProtocolCore.Extend;
using ProtocolCore.Protocol;
using ProtocolCore.Serialize;
using ServiceTools.Seralize;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Test
{
    public class BinaryBufferTest
    {
        static BinaryBufferTest()
        {
            BitSerializer.Start();
        }
        enum testEnum
        {
            a = 1,
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
        [Test]
        public void TestList()
        {
            List<string> slist = new List<string>() { "12312341", "dxfsada", "cvv12131", "cvxzcsjkd1" };
            TestList(slist);
            List<int> ilist = new List<int>() { 12312341, 12513153, 843132, 843621 };
            TestList(ilist);
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

        [Test]
        public void TestSeralize()
        {
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

            byte[] bytes = BitSerializer.Seralize(class1);
            Class1 class11 = BitSerializer.Deseralize(typeof(Class1), bytes) as Class1;
            Assert.IsTrue(class1.Equals(class11));
        }
        /// <summary>
        /// 扩展模式
        /// </summary>
        [Test]
        public void TestSerializeExtend()
        {
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

        [ProtocolSerializable]
        public class Iterations : ProtoData
        {
            [ProtocolSerializeField]
            public List<Class1> class1s = new List<Class1>();
        }

        [Test]
        public void TestSeralizePerformance()
        {
            int dataCount = 1000;
            int iteration = 1000;


            Class1 class1 = new Class1();
            class1.ArrayValue = new int[] { 1, 2, 3, 4, 5 };
            class1.Class2.s = "classsedwqdsavdddddddddddddddddsdasdasddddddddddddddddd"
                              + "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd"
                              + "sdasddddddddddddddddd" + "asddddddddddddddd"
                              + "dasddddddddddddddddddddddxcvxcvxcvxcv"
                              + "cxvcxxxxxxxxxxxxxxxxxxxxxxxxczxcsadasdasdasdssssss";
            class1.b = true;
            class1.i = 12312;
            for (int i = 0; i < dataCount; i++)
            {
                class1.listValue.Add(i);
            }
            /*     class1.Dictionary[1] = "dsasdasd";
                 class1.Dictionary[2] = "ddscccxz";*/
            class1.s = "192.168.1.37";

            Iterations iterations = new Iterations();
            for (int i = 0; i < iteration; i++)
            {
                iterations.class1s.Add(class1);
            }

            #region 序列化反序列化性能

            Stopwatch watch = new Stopwatch();
            byte[] bytes = null; Iterations iterations1;
            byte[] CompBbytes;
            {
                watch.Restart();
                bytes = BitSerializer.Seralize(iterations);
                watch.Stop();
                UnityEngine.Debug.Log("Bit seralizeTime:" + watch.ElapsedMilliseconds);
                // string s = ProtocolSerialize.DebugSeralizeStr(bytes);
                watch.Restart();
                iterations1 = BitSerializer.Deseralize(typeof(Iterations), bytes) as Iterations;
                watch.Stop();
                UnityEngine.Debug.Log("Bit DeseralizeTime:" + watch.ElapsedMilliseconds);


                #region 大小

                UnityEngine.Debug.Log("Protocol序列化:" + bytes.Length);
                CompBbytes = CompressBase.CompressBytes(bytes);
                UnityEngine.Debug.Log("压缩后:" + CompBbytes.Length);

                #endregion

            }
            {
                watch.Restart();
                ExtendProtocolSerialize.Enable();
                bytes = ProtocolSerialize.Seralize(iterations);
                watch.Stop();
                UnityEngine.Debug.Log("seralizeTime:" + watch.ElapsedMilliseconds);
                // string s = ProtocolSerialize.DebugSeralizeStr(bytes);
                watch.Restart();
                iterations1 = ProtocolSerialize.Deseralize(typeof(Iterations), bytes) as Iterations;
                watch.Stop();
                UnityEngine.Debug.Log("DeseralizeTime:" + watch.ElapsedMilliseconds);


                #region 大小

                UnityEngine.Debug.Log("Protocol序列化:" + bytes.Length);
                CompBbytes = CompressBase.CompressBytes(bytes);
                UnityEngine.Debug.Log("压缩后:" + CompBbytes.Length);

                #endregion
            }
            {
                watch.Restart();
                var json = UnityEngine.JsonUtility.ToJson(iterations);

                watch.Stop();
                UnityEngine.Debug.Log("json seralizeTime:" + watch.ElapsedMilliseconds);
                // string s = ProtocolSerialize.DebugSeralizeStr(bytes);
                watch.Restart();
                iterations1 = UnityEngine.JsonUtility.FromJson<Iterations>(json);
                watch.Stop();
                UnityEngine.Debug.Log("json DeseralizeTime:" + watch.ElapsedMilliseconds);

            }
            #endregion






        }

        #region Internal
        private void TestStringSeralize(string value)
        {
            byte[] bs = BitSerializer.Seralize(value);
            object obj = BitSerializer.Deseralize(value.GetType(), bs);
            string newvalue = obj as string;
            Assert.AreEqual(value, newvalue);
        }
        private void TestValueSeralize(ValueType value)
        {
            byte[] bs = BitSerializer.Seralize(value);
            Debug.WriteLine(value + "序列化后字节数" + bs.Length);
            object newvalue = BitSerializer.Deseralize(value.GetType(), bs);
            Assert.AreEqual(value, newvalue);
        }

        private void TestArray(Array array)
        {
            byte[] bs = BitSerializer.Seralize(array);
            object obj = BitSerializer.Deseralize(array.GetType(), bs);
            Array newarray = obj as Array;
            Assert.AreEqual(array.Length, newarray.Length);
            for (int i = 0; i < array.Length; i++)
            {
                Assert.AreEqual(array.GetValue(i), newarray.GetValue(i));
            }
        }
        private void TestList(IList list)
        {
            byte[] bs = BitSerializer.Seralize(list);
            IList newlist = BitSerializer.Deseralize(list.GetType(), bs) as IList;
            for (int i = 0; i < list.Count; i++)
            {
                Assert.AreEqual(list[i], newlist[i]);
            }
        }

        private void TestDictionary(IDictionary dic)
        {
            byte[] bs = BitSerializer.Seralize(dic);
            Dictionary<int, string> obj = BitSerializer.Deseralize(dic.GetType(), bs) as Dictionary<int, string>;
            Assert.AreEqual(obj.Count, dic.Count);
            foreach (KeyValuePair<int, string> pair in obj)
            {
                Assert.AreEqual(dic[pair.Key], pair.Value);
            }
        }
        #endregion
    }

}

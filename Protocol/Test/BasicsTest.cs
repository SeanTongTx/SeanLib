using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Unity.Collections;
using UnityEngine;

public class BasicsTest
{
    byte[] array;
    List<byte> list;
    NativeArray<byte> narray;
    MemoryStream steam;
    const int SampleDataCount = 100*10000;
    const int iteration = 1000*10000;
    private void InitData()
    {
        narray = new NativeArray<byte>(SampleDataCount,Allocator.Persistent);
       array = new byte[SampleDataCount];
        for (int i = 0; i < SampleDataCount; i++)
        {
            array[i] = 1;
        }
        list = new List<byte>(SampleDataCount);
        for (int i = 0; i < SampleDataCount; i++)
        {
            list.Add(1);
        }
        if (steam!=null)
        {
            steam.Dispose();
            steam = null;
        }
        steam = new MemoryStream(SampleDataCount);
        BinaryWriter writer = new BinaryWriter(steam);
        for (int i = 0; i < SampleDataCount; i++)
        {
            writer.Write(1);
        }
    }
    [Test]
    public void TestCollectionOperationSpeed()
    {
        UnityEngine.Debug.Log("测试几个简单集合的操作速度");
        Stopwatch watch = new Stopwatch();
        long count = 0;

        UnityEngine.Debug.Log("-------------赋值");
        /*赋值*/
        narray = new NativeArray<byte>(SampleDataCount, Allocator.Persistent);
        watch.Start();
        for (int i = 0; i < SampleDataCount; i++)
        {
            narray[i] = 1;
        }
        watch.Stop();
        UnityEngine.Debug.Log("narray[i] = 1" + watch.ElapsedMilliseconds);
        array = new byte[SampleDataCount];
        watch.Start();
        for (int i = 0; i < SampleDataCount; i++)
        {
            array[i] = 1;
        }
        watch.Stop();
        UnityEngine.Debug.Log("array[i] = 1" + watch.ElapsedMilliseconds);
        list = new List<byte>(SampleDataCount);
        watch.Start();
        for (int i = 0; i < SampleDataCount; i++)
        {
            list.Add(1);
        }
        watch.Stop();
        UnityEngine.Debug.Log("list.Add(1)" + watch.ElapsedMilliseconds);
        if (steam != null)
        {
            steam.Dispose();
            steam = null;
        }
        steam = new MemoryStream(SampleDataCount);
        BinaryWriter writer = new BinaryWriter(steam);

        watch.Start();
        for (int i = 0; i < SampleDataCount; i++)
        {
            writer.Write(1);
        }
        watch.Stop();
        UnityEngine.Debug.Log("writer.Write(1)" + watch.ElapsedMilliseconds);

        UnityEngine.Debug.Log("-------------长度");
        /*长度*/
        watch.Start();
        for (int i = 0; i < iteration; i++)
        {
            count = narray.Length;
        }
        watch.Stop();
        UnityEngine.Debug.Log("narray.lenght:" + watch.ElapsedMilliseconds);

        watch.Start();
        for (int i = 0; i < iteration; i++)
        {
            count = array.Length;
        }
        watch.Stop();
        UnityEngine.Debug.Log("Array.lenght:" + watch.ElapsedMilliseconds);

        watch.Start();
        for (int i = 0; i < iteration; i++)
        {
            count = list.Count;
        }
        watch.Stop();
        UnityEngine.Debug.Log("list.Count:" + watch.ElapsedMilliseconds);

        watch.Start();
        for (int i = 0; i < iteration; i++)
        {
            count = steam.Length;
        }
        watch.Stop();
        UnityEngine.Debug.Log("steam.Length:" + watch.ElapsedMilliseconds);
        UnityEngine.Debug.Log("-------------取值");
        /*取值*/
        watch.Start();
        for (int i = 0; i < SampleDataCount; i++)
        {
            byte t = narray[i];
        }
        watch.Stop();
        UnityEngine.Debug.Log("narray[i]:" + watch.ElapsedMilliseconds);
        watch.Start();
        for (int i = 0; i < SampleDataCount; i++)
        {
            byte t = array[i];
        }
        watch.Stop();
        UnityEngine.Debug.Log("Array[i]:" + watch.ElapsedMilliseconds);

        watch.Start();
        for (int i = 0; i < SampleDataCount; i++)
        {
            byte t = list[i];
        }
        watch.Stop();
        UnityEngine.Debug.Log("list[i]:" + watch.ElapsedMilliseconds);

        BinaryReader reader = new BinaryReader(steam);
        steam.Position = 0;
        watch.Start();
        for (int i = 0; i < SampleDataCount; i++)
        {
            byte t = reader.ReadByte();
        }
        watch.Stop();
        UnityEngine.Debug.Log("reader.ReadByte:" + watch.ElapsedMilliseconds);


        narray.Dispose();
    }
}

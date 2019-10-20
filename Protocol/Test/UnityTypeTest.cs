using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtocolCore;
public class UnityTypeTest 
{
    enum testenum
    { }

    [ProtocolSerializable]
    class UnityTypeClass
    {
        [ProtocolSerializeField]
        public Vector2 v2;
        [ProtocolSerializeField]
        public Vector2 v3;
        [ProtocolSerializeField]
        public Vector2 v4;
        [ProtocolSerializeField]
        public Color c;
        [ProtocolSerializeField]
        public Color32 c32;
        [ProtocolSerializeField]
        public Rect rect;

        [ProtocolSerializeField]
        public List<Vector2> v2List = new List<Vector2>();
        [ProtocolSerializeField]
        public Color[] cArray;

        [ProtocolSerializeField]
        public Quaternion quaternion;
        public AnimationCurve curve;
    }
    struct strt
    {
        public int a;
        public float f;
    }
    [Test]
    public void IsValueType()
    {
        List<Type> types = new List<Type>();
        types.Add(typeof(Vector2));
        types.Add(typeof(Vector3));
        types.Add(typeof(Vector4));
        types.Add(typeof(Color));
        types.Add(typeof(Color32));
        types.Add(typeof(Rect));
        types.Add(typeof(Quaternion));
        types.Add(typeof(AnimationCurve));
        types.Add(typeof(strt));
        types.Add(typeof(string));

        foreach (var t in types)
        {
            Debug.Log(t.FullName + ":" + t.IsValueType);
        }
    }
    [Test]
    public void IsPrimitive()
    {
        List<Type> types = new List<Type>();
        types.Add(typeof(int));
        types.Add(typeof(float));
        types.Add(typeof(bool));
        types.Add(typeof(double));
        types.Add(typeof(string));
        types.Add(typeof(testenum));
        types.Add(typeof(List<int>));
        types.Add(typeof(int[]));
        types.Add(typeof(DateTime));
        types.Add(typeof(Enum));
        types.Add(typeof(Vector2));
        types.Add(typeof(Vector3));
        types.Add(typeof(Vector4));
        types.Add(typeof(Color));
        types.Add(typeof(Color32));
        types.Add(typeof(Rect));
        types.Add(typeof(Quaternion));
        types.Add(typeof(AnimationCurve));
        types.Add(typeof(strt));

        foreach (var t in types)
        {
            Debug.Log(t.FullName + ":" + t.IsPrimitive);
        }
    }


    [Test]
    public void TestUnityTypeClass()
    {
        ProtocolCore.Extend.ExtendProtocolSerialize.Enable();
        ProtocolCore.Extend.ExtendUnityProtocol.Enable();
        UnityTypeClass ut = new UnityTypeClass();
        ut.v2 = Vector2.one * 2;
        ut.v3 = Vector3.one * 2;
        ut.v4 = Vector2.one * 2;
        ut.rect = new Rect(100,100,50,50);
        ut.c = Color.red;
        ut.c32 = Color.yellow;
        ut.quaternion =new Quaternion(1,2,3,4);
        ut.v2List = new List<Vector2>() { Vector2.one * 2, Vector2.one * 3, Vector2.one * 4 };
        ut.cArray = new Color[] { Color.red,Color.yellow,Color.blue,Color.cyan,Color.green };

        byte[] bs= ProtocolSerialize.Seralize(ut);
        UnityTypeClass newut = ProtocolSerialize.Deseralize<UnityTypeClass>(bs);
        Assert.AreEqual(ut.v2, newut.v2);
        Assert.AreEqual(ut.v3, newut.v3);
        Assert.AreEqual(ut.v4, newut.v4);
        Assert.AreEqual(ut.rect, newut.rect);
        Assert.AreEqual(ut.c, newut.c);
        Assert.AreEqual(ut.c32, newut.c32);
        Assert.AreEqual(ut.quaternion, newut.quaternion);
        for (int i = 0; i < ut.v2List.Count; i++)
        {
            Assert.AreEqual(ut.v2List[i], newut.v2List[i]);
        }
        for (int i = 0; i < ut.cArray.Length; i++)
        {
            Assert.AreEqual(ut.cArray[i], newut.cArray[i]);
        }
    }
}

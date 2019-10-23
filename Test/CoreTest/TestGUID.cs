using NUnit.Framework;
using SeanLib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGUID
{
    [Test]
    public void TestGUID_Identity_10million()
    {
        HashSet<long> idset = new HashSet<long>();
        for (int i = 0; i < 1000*10000; i++)
        {
            long id= GUIDHelper.Identity();
            if(idset.Contains(id))
            {
                Assert.Fail("重复了 index:" + i);
            }
            idset.Add(id);
        }
    }
}

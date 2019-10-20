using NUnit.Framework;
using ProtocolCore.Compression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GZipTest
{
    [Test]
    public void TestCompress()
    {
        byte[] bs = new byte[] { 1, 2, 3, 1, 23, 12, 41, 31, 3, 1, 21, 1, 1, 21, 1, 21 };
        Debug.Log(bs.Length);
        byte[] outbys = CompressBase.CompressBytes(bs);
        Debug.Log(outbys.Length);
        outbys = CompressBase.DecompressBytes(outbys);
        Debug.Log(outbys.Length);

    }

}

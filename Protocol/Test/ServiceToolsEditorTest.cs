using ServiceTools.Security.Algorithm;

using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class ServiceToolsEditorTest
{
    [Test]
    public void TestAes()
    {
        AESCode aes = new AESCode();
        aes.Key = "123456789";
        string words = "zxczxczxczxczxczxczxczxcxzczxczxczx";
        string result = aes.Encrypt(words);
        Debug.Log("Encrypt =>" + result);
        string rr = aes.Decrypt(result);
        Debug.Log("Decrypt =>" + rr);
    }

    [Test]
    public void TestRSA()
    {
        string keyPublic, keyPrivate;
        string words = "zxczxczxczxczxczxczxczxcxzczxczxczx";
        string encodeWords, decode;
        if (RSACode.CreateKey(out keyPublic, out keyPrivate))
        {
            encodeWords = RSACode.Encrypt(keyPublic, words);
            Debug.Log("Encrypt =>" + encodeWords);
            decode = RSACode.Decrypt(keyPrivate, encodeWords);
            Debug.Log("Decrypt =>" + decode);
        }
        else
        {
            Assert.Fail("创建秘钥失败");
        }

    }
    /*[Test]
    public void ServiceToolsEditorTestSimplePasses() {
        // Use the Assert class to test conditions.
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
    public IEnumerator ServiceToolsEditorTestWithEnumeratorPasses() {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        yield return null;
    }*/
}

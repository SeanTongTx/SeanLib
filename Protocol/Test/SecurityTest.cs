using System;
using UnityEngine;
namespace Test
{
    using NUnit.Framework;
    using ServiceTools;
    using ServiceTools.Security.Algorithm;
    
    public class SecurityTest
    {
        [Test]
        public void TestAes()
        {
            AESCode aes = new AESCode();
            aes.Key = "123456789";

            string encode = aes.Encrypt(aes.Key);
             Debug.Log("Encrypt =>" + encode);
            string decode = aes.Decrypt(encode);
            Debug.Log("Decrypt =>" + decode);

            Assert.AreEqual(aes.Key, decode);
            encode = aes.Encrypt(aes.Key);
            Debug.Log("Encrypt =>" + encode);
            decode = aes.Decrypt(encode);
            Debug.Log("Decrypt =>" + decode);
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
                Assert.AreEqual(words, decode);
            }
            else
            {
                Assert.Fail("创建秘钥失败");
            }
        }

        [Test]
        public void TestMD5()
        {
            string words = "zxczxczxczxczxczxczxczxcxzczxczxczx";
            string md5 = HashTools.MD5Encrypt(words);
            Debug.Log("md5 =>" + md5);
        }

        [Test]
        public void TestSHA1()
        {
            string words = "zxczxczxczxczxczxczxczxcxzczxczxczx";
            string sha1 = HashTools.SHA1Encrypt(words);
            Debug.Log("sha1 =>" + sha1);
        }
    }
}

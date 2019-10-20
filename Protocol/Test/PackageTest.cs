using System;

namespace Test
{
    using NUnit.Framework;
    using Protocol;
    using Protocol.Package;

    using ProtocolCore;
    using ProtocolCore.Extend;
    using ProtocolCore.Package;

    public class PackageTest
    {
        static PackageTest()
        {
            ExtendProtocolSerialize.Enable();
        }
        [Test]
        public void TestPackAndUnPack()
        {
            Class1 class1 = new Class1();
            class1.ArrayValue = new int[] { 1, 2, 3, 4, 5 };
            class1.Class2.s = "classsedwqdsavcxvcxxxxxxxxxxxxxxxxxxdsaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                              + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                              + "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
                              + "aaaaxxxxxxczxcsadasdasdasdssssss";
            class1.b = true;
            class1.i = 12312;
            class1.listValue.Add(132);
            class1.Dictionary[1] = "dsasdasd";
            class1.Dictionary[2] = "ddscccxz";
            class1.s = "192.168.1.37";
            ProtocolPackage[] packages = Packager.Pack(class1, 1024);
            Assert.AreEqual(packages.Length, 1);
            byte[] bytes = Packager.UnPack(packages);
            bytes = ProtocolSerialize.Seralize(packages[0]);
            ProtocolPackage package = ProtocolSerialize.Deseralize(typeof(ProtocolPackage), bytes) as ProtocolPackage;
            Assert.AreNotEqual(package, null);
        }
    }
}

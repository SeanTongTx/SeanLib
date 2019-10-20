using ProtocolCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    [ProtocolSerializable]
    public class Class3
    {
        public string s;

        public int i;

        public Class4 class4;

        public override bool Equals(object obj)
        {
            Class3 c31 = obj as Class3;
            return s == c31.s && i == c31.i && class4.Equals(c31.class4);
        }
    }
}

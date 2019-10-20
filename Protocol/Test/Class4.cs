using ProtocolCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    [ProtocolSerializable]
    public class Class4
   {
       public float i;

       public bool b;

       public override bool Equals(object obj)
       {
           Class4 c4 = obj as Class4;
           return i == c4.i && b == c4.b;
       }
   }
}

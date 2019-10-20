using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Package
{
    using ProtocolCore;
    using ProtocolCore.Package;

    [ProtocolSerializable]
    public class RPCPackage : ProtocolPackage
    {
        [ProtocolSerializeField]
        public string Method = "Testtttttttttt";
    }
}

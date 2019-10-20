namespace Protocol
{
    using System;

    using ProtocolCore;

    [Serializable]
    [ProtocolSerializable]
    public class Class2 : ProtoData
    {
        [ProtocolSerializeField]
        public string s = "Class2";

        public int i = 2;

        [ProtocolSerializeField]
        public bool b;

        public override bool Equals(object obj)
        {
            if (obj is Class2)
            {
                Class2 other = obj as Class2;
                return other.i == i && b == other.b;
            }
            return base.Equals(obj);
        }
    }
}

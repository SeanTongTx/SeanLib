// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="">
//   
// </copyright>
// <summary>
//   The class 1.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Protocol
{
    using System;
    using System.Collections.Generic;

    using ProtocolCore;

    /// <summary>
    ///     The class 1.
    /// </summary>
    [Serializable]
    [ProtocolSerializable]
    public class Class1 : ProtoData
    {
        #region Fields

        /// <summary>
        ///     The array value.
        /// </summary>
        [ProtocolSerializeField]
        public int[] ArrayValue = { };

        /// <summary>
        ///     The class 2.
        /// </summary>
        [ProtocolSerializeField]
        public Class2 Class2 = new Class2();

        /// <summary>
        ///     The dictionary.
        /// </summary>
        [ProtocolSerializeField]
        public Dictionary<int, string> Dictionary = new Dictionary<int, string>();

        /// <summary>
        ///     The b.
        /// </summary>
        [ProtocolSerializeField]
        public bool b;

        /// <summary>
        ///     The i.
        /// </summary>
        [ProtocolSerializeField]
        public int i = 1;

        public object obj;

        /// <summary>
        ///     The list value.
        /// </summary>
        [ProtocolSerializeField]
        public List<int> listValue = new List<int>();

        /// <summary>
        ///     The s.
        /// </summary>
        [ProtocolSerializeField]
        public string s = "Class1";

        [ProtocolSerializeField]
        public double d = 123d;

        [ProtocolSerializeField]
        public float f = 1223f;
        #endregion

        public override bool Equals(object obj)
        {
            if (obj is Class1)
            {
                Class1 other = obj as Class1;
                for (int ii = 0; ii < ArrayValue.Length; ii++)
                {
                    if (ArrayValue[ii] != other.ArrayValue[ii]) return false;
                }
                foreach (KeyValuePair<int, string> pair in Dictionary)
                {
                    if (other.Dictionary[pair.Key] != pair.Value) return false;
                }
                for (int ii = 0; ii < listValue.Count; ii++)
                {
                    if (listValue[ii] != other.listValue[ii]) return false;
                }
                return Class2.Equals(other.Class2) && b == other.b && i == other.i && s == other.s && d == other.d && f == other.f;
            }
            return base.Equals(obj);
        }
    }
}
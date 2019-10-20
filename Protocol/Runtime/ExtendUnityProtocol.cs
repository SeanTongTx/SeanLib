namespace ProtocolCore.Extend
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using ProtocolCore;
    using ProtocolCore.Serialize;
    using UnityEngine;
    public  class ExtendUnityProtocol:ExtendSerializer
    {
        public static ExtendUnityProtocol instance = new ExtendUnityProtocol();
        public static void Enable()
        {
            ProtocolSerialize.RegistSerialize(instance);
        }
        public static void Unable()
        {
            ProtocolSerialize.UnregistSerialize(instance);
        }
        public override byte[] Serialize(object arg)
        {
            if (arg is Vector2)
            {
                Vector2 v2 = (Vector2)arg;
                float[] f2 = new float[] { v2.x, v2.y };
                return valueBytes_Array(f2);
            }
            else if (arg is Vector3)
            {
                Vector3 v3 = (Vector3)arg;
                float[] f3 = new float[] { v3.x, v3.y, v3.z };
                return valueBytes_Array(f3);
            }
            else if (arg is Vector4)
            {
                Vector4 v4 = (Vector4)arg;
                float[] f4 = new float[] { v4.x, v4.y, v4.z, v4.w };
                return valueBytes_Array(f4);
            }
            else if (arg is Rect)
            {
                Rect v4 = (Rect)arg;
                float[] f4 = new float[] { v4.x, v4.y, v4.width, v4.height };
                return valueBytes_Array(f4);
            }
            else if (arg is Quaternion)
            {
                Quaternion q = (Quaternion)arg;
                float[] f4 = new float[] { q.x, q.y, q.z, q.w };
                return valueBytes_Array(f4);
            }
            else if (arg is Color)
            {
                Color c = (Color)arg;
                float[] f4 = new float[] { c.r, c.g, c.b, c.a };
                return valueBytes_Array(f4);
            }
            else if (arg is Color32)
            {
                Color32 c = (Color32)arg;
                byte[] b4 = new byte[] { c.r, c.g, c.b, c.a };
                return valueBytes_Array(b4);
            }
            else if (arg is AnimationCurve)
            {
                return new byte[] { };
            }
            return null;
        }
        public override object Deserialize(Type arg1, byte[] arg2)
        {
            if (arg1 == typeof(Vector2))
            {
                float[] f2 = BytesValue_Array(typeof(float[]), arg2) as float[];
                return new Vector2(f2[0], f2[1]);
            }
            else if (arg1 == typeof(Vector3))
            {
                float[] f3 = BytesValue_Array(typeof(float[]), arg2) as float[];
                return new Vector3(f3[0], f3[1], f3[2]);
            }
            else if (arg1 == typeof(Vector4))
            {
                float[] f4 = BytesValue_Array(typeof(float[]), arg2) as float[];
                return new Vector4(f4[0], f4[1], f4[2], f4[3]);
            }
            else if (arg1 == typeof(Rect))
            {
                float[] f4 = BytesValue_Array(typeof(float[]), arg2) as float[];
                return new Rect(f4[0], f4[1], f4[2], f4[3]);
            }
            else if (arg1 == typeof(Quaternion))
            {
                float[] f4 = BytesValue_Array(typeof(float[]), arg2) as float[];
                return new Quaternion(f4[0], f4[1], f4[2], f4[3]);
            }
            else if (arg1 == typeof(Color))
            {
                float[] f4 = BytesValue_Array(typeof(float[]), arg2) as float[];
                return new Color(f4[0], f4[1], f4[2], f4[3]);
            }
            else if (arg1 == typeof(Color32))
            {
                byte[] b4 = BytesValue_Array(typeof(byte[]), arg2) as byte[];
                return new Color32(b4[0], b4[1], b4[2], b4[3]);
            }
            else if (arg1 == typeof(AnimationCurve))
            {
                return AnimationCurve.EaseInOut(0, 0, 1, 1);
            }
            return null;
        }
        public override bool WriteValueList(BinaryWriter Writer, IList list)
        {
            Type type = list.GetType();
            Type dataType = type.GetGenericArguments()[0];
            if(dataType==typeof(Vector2))
            {
                writeListBytes(Writer, list as List<Vector2>);
            }
            else if (dataType == typeof(Vector3))
            {
                writeListBytes(Writer, list as List<Vector3>);
            }
            else if (dataType == typeof(Vector4))
            {
                writeListBytes(Writer, list as List<Vector4>);
            }
            else if (dataType == typeof(Color))
            {
                writeListBytes(Writer, list as List<Color>);
            }
            else if (dataType == typeof(Color32))
            {
                writeListBytes(Writer, list as List<Color32>);
            }
            else if (dataType == typeof(Rect))
            {
                writeListBytes(Writer, list as List<Rect>);
            }
            else if (dataType == typeof(Quaternion))
            {
                writeListBytes(Writer, list as List<Quaternion>);
            }
            else
            {
                return false;
            }
            return true;
        }
        public override bool WriteValueArray(BinaryWriter Writer, Array array)
        {
            Type dataType = array.GetType().GetElementType();
            if (dataType == typeof(Vector2))
            {
                writeArrayBytes(Writer, array as Vector2[]);
            }
            else if (dataType == typeof(Vector3))
            {
                writeArrayBytes(Writer, array as Vector3[]);
            }
            else if (dataType == typeof(Vector4))
            {
                writeArrayBytes(Writer, array as Vector4[]);
            }
            else if (dataType == typeof(Color))
            {
                writeArrayBytes(Writer, array as Color[]);
            }
            else if (dataType == typeof(Color32))
            {
                writeArrayBytes(Writer, array as Color32[]);
            }
            else if (dataType == typeof(Rect))
            {
                writeArrayBytes(Writer, array as Rect[]);
            }
            else if (dataType == typeof(Quaternion))
            {
                writeArrayBytes(Writer, array as Quaternion[]);
            }
            else
            {
                return false;
            }
            return true;
        }

        public override IList ReadValueList(Type type, BinaryReader reader)
        {
            IList List = null;
            if (type == typeof(Quaternion))
            {
                List = new List<Quaternion>((int)reader.BaseStream.Length / 16);
                ReadListBytes(List as List<Quaternion>, reader);
            }
            else if (type == typeof(Rect))
            {
                List = new List<Rect>((int)reader.BaseStream.Length / 16);
                ReadListBytes(List as List<Rect>, reader);
            }
            else if (type == typeof(Vector2))
            {
                List = new List<Vector2>((int)reader.BaseStream.Length / 8);
                ReadListBytes(List as List<Vector2>, reader);
            }
            else if (type == typeof(Vector3))
            {
                List = new List<Vector3>((int)reader.BaseStream.Length / 12);
                ReadListBytes(List as List<Vector3>, reader);
            }
            else if (type == typeof(Vector4))
            {
                List = new List<Vector4>((int)reader.BaseStream.Length / 16);
                ReadListBytes(List as List<Vector4>, reader);
            }
            else if (type == typeof(Color))
            {
                List = new List<Color>((int)reader.BaseStream.Length / 16);
                ReadListBytes(List as List<Color>, reader);
            }
            else if (type == typeof(Color))
            {
                List = new List<Color32>((int)reader.BaseStream.Length / 4);
                ReadListBytes(List as List<Color32>, reader);
            }
            return List;
        }

        #region encode
        private void writeListBytes(BinaryWriter Writer, List<Quaternion> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Writer.Write(list[i].x);
                Writer.Write(list[i].y);
                Writer.Write(list[i].z);
                Writer.Write(list[i].w);
            }
        }
        private void writeListBytes(BinaryWriter Writer, List<Rect> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Writer.Write(list[i].x);
                Writer.Write(list[i].y);
                Writer.Write(list[i].width);
                Writer.Write(list[i].height);
            }
        }
        private void writeListBytes(BinaryWriter Writer, List<Color32> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Writer.Write(list[i].r);
                Writer.Write(list[i].g);
                Writer.Write(list[i].b);
                Writer.Write(list[i].a);
            }
        }
        private void writeListBytes(BinaryWriter Writer, List<Color> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Writer.Write(list[i].r);
                Writer.Write(list[i].g);
                Writer.Write(list[i].b);
                Writer.Write(list[i].a);
            }
        }
        private void writeListBytes(BinaryWriter Writer, List<Vector2> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Writer.Write(list[i].x);
                Writer.Write(list[i].y);
            }
        }
        private void writeListBytes(BinaryWriter Writer, List<Vector3> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Writer.Write(list[i].x);
                Writer.Write(list[i].y);
                Writer.Write(list[i].z);
            }
        }
        private void writeListBytes(BinaryWriter Writer, List<Vector4> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Writer.Write(list[i].x);
                Writer.Write(list[i].y);
                Writer.Write(list[i].z);
                Writer.Write(list[i].w);
            }
        }

        private void writeArrayBytes(BinaryWriter Writer, Quaternion[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Writer.Write(list[i].x);
                Writer.Write(list[i].y);
                Writer.Write(list[i].z);
                Writer.Write(list[i].w);
            }
        }
        private void writeArrayBytes(BinaryWriter Writer, Rect[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Writer.Write(list[i].x);
                Writer.Write(list[i].y);
                Writer.Write(list[i].width);
                Writer.Write(list[i].height);
            }
        }
        private void writeArrayBytes(BinaryWriter Writer, Color32[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Writer.Write(list[i].r);
                Writer.Write(list[i].g);
                Writer.Write(list[i].b);
                Writer.Write(list[i].a);
            }
        }
        private void writeArrayBytes(BinaryWriter Writer, Color[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Writer.Write(list[i].r);
                Writer.Write(list[i].g);
                Writer.Write(list[i].b);
                Writer.Write(list[i].a);
            }
        }
        private void writeArrayBytes(BinaryWriter Writer, Vector2[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Writer.Write(list[i].x);
                Writer.Write(list[i].y);
            }
        }
        private void writeArrayBytes(BinaryWriter Writer, Vector3[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Writer.Write(list[i].x);
                Writer.Write(list[i].y);
                Writer.Write(list[i].z);
            }
        }
        private void writeArrayBytes(BinaryWriter Writer, Vector4[] list)
        {
            for (int i = 0; i < list.Length; i++)
            {
                Writer.Write(list[i].x);
                Writer.Write(list[i].y);
                Writer.Write(list[i].z);
                Writer.Write(list[i].w);
            }
        }
        #endregion
        #region decode
        private static void ReadListBytes(List<Color32> list, BinaryReader reader)
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                list.Add(new Color32(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte()));
            }
        }
        private static void ReadListBytes(List<Color> list, BinaryReader reader)
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                list.Add(new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            }
        }
        private static void ReadListBytes(List<Vector4> list, BinaryReader reader)
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                list.Add(new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            }
        }
        private static void ReadListBytes(List<Vector3> list, BinaryReader reader)
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                list.Add(new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            }
        }
        private static void ReadListBytes(List<Vector2> list, BinaryReader reader)
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                list.Add(new Vector2(reader.ReadSingle(), reader.ReadSingle()));
            }
        }
        private static void ReadListBytes(List<Rect> list, BinaryReader reader)
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                list.Add(new Rect(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            }
        }
        private static void ReadListBytes(List<Quaternion> list, BinaryReader reader)
        {
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                list.Add(new Quaternion(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            }
        }
        #endregion
    }
}

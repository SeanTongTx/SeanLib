using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace SeanLib.Core
{
    public static partial class TypeHelper
    {
        public static object DefaultValue(Type type)
        {
            if (type == null)
            {
                return null;
            }
            if (type.IsValueType)
            {
                if (type.Equals(typeof(int)))
                {
                    return 0;
                }
                if (type.Equals(typeof(bool)))
                {
                    return false;
                }
                if (type.Equals(typeof(float)))
                {
                    return 0f;
                }
                if (type.Equals(typeof(double)))
                {
                    return 0d;
                }
                if (type.Equals(typeof(long)))
                {
                    return 0L;
                }
                ///UnityType
                if (type.Equals(typeof(Color)))
                {
                    return Color.white;
                }
                if (type.Equals(typeof(Vector2)))
                {
                    return Vector2.zero;
                }
                if (type.Equals(typeof(Vector3)))
                {
                    return Vector3.zero;
                }
                if (type.Equals(typeof(Vector4)))
                {
                    return Vector4.zero;
                }
                if (type.Equals(typeof(Quaternion)))
                {
                    return Quaternion.identity;
                }
                if (type.Equals(typeof(Rect)))
                {
                    return Rect.zero;
                }
                return Activator.CreateInstance(type);
            }
            else if (type.Equals(typeof(string)))
            {
                return string.Empty;
            }
            else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                return ((UnityEngine.Object)null);
            }
            else if (type.IsArray)
            {
                Type datatype = type.GetElementType();
                return Array.CreateInstance(datatype, 0);
            }
            else
            {
                ConstructorInfo constructor = type.GetConstructor(new Type[] { });
                if (constructor != null)
                {
                    return Activator.CreateInstance(type, true);
                }
            }
            return null;
        }
        public static Type GetType(string typeFullname, Assembly asmb = null)
        {
            if (asmb != null)
            {
                return asmb.GetType(typeFullname);
            }
            Type type = Type.GetType(typeFullname);
            //
            if (type == null)
            {
                type = Assembly.GetAssembly(typeof(UnityEngine.Object)).GetType(typeFullname);
            }
            return type;
        }
    }
}
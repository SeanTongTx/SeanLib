using ServiceTools.Reflect;
using System;
using System.Collections.Generic;

using UnityEngine;
namespace SeanLib.Core
{

    [ExecuteInEditMode]
    public class Instance : MonoBehaviour
    {
        /// <summary>
        /// The instances.
        /// </summary>
        public static Dictionary<Type, Component> Instances = new Dictionary<Type, Component>();
        [Tooltip("调用组件的Init方法")]
        public bool Init;

        [Tooltip("单例组件")]
        public Component component;

        public void Awake()
        {
            if (component)
            {
                Instances[component.GetType()] = component;

                if (Init)
                {
                    ReflecTool.InvokeMethod(component, "Init", null);
                }
            }
        }

        public void OnValidate()
        {
            Awake();
        }
        /// <summary>
        /// The get instance.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T GetInstance<T>() where T : Component
        {
            Component value = null;
            Instances.TryGetValue(typeof(T), out value);
            return value as T;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus
{
    public class SelectComponentWindow : SelectWindow<Component>
    {
        public static void Show<T>(GameObject obj, string controlId) where T : Component
        {
            var coms = obj.GetComponents<T>();
            var comlist = new List<Component>();
            comlist.Add(null);
            foreach (var item in coms)
            {
                comlist.Add(item as Component);
            }
            Show(comlist, controlId);
        }
    }
}
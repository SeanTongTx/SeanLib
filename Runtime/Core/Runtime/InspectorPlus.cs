using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SeanLib.Core
{
    public class InspectorPlus
    {
        //Custom PropertyDrawer
        public abstract class InspectorPlusAttribute : PropertyAttribute
        { }

        /// <summary>
        /// 用在list上
        /// </summary>
        public class Orderable : PropertyAttribute
        {

        }
        /// <summary>
        /// 用在list上
        /// </summary>
        public class Singleton : PropertyAttribute
        {

        }
        /// <summary>
        /// 只读
        /// </summary>
        public class ReadOnly : InspectorPlusAttribute
        {

        }
        /// <summary>
        /// GUID生成按钮
        /// </summary>
        [AttributeUsage(AttributeTargets.Field)]
        public class GUID : InspectorPlusAttribute
        {
        }
        [AttributeUsage(AttributeTargets.Field)]
        public class TypeName : InspectorPlusAttribute
        {
        }
        /// <summary>
        /// SceneReference 显示设置
        /// </summary>
        public class SceneRefAtt : InspectorPlusAttribute
        {
            public bool ShowFieldStr = true;
            public bool ShowRefType = false;
            public bool ShowDetail = true;
        }
        public class HelpBox : Attribute
        {
            public string Msg;
            public int MessageType = 1;
        }

        public class SetProperty : InspectorPlusAttribute
        {
            public string Name { get; private set; }
            public int id;

            public SetProperty(string name)
            {
                this.id = UnityEngine.Random.Range(0, int.MaxValue);
                this.Name = name;
            }
        }
        public class GroupBegin : InspectorPlusAttribute
        {
            public string Name { get; private set; }
            public GroupBegin(string name)
            {
                this.Name = name;
            }
        }
        public class GroupEnd : InspectorPlusAttribute
        {
            public GroupEnd()
            {
            }
        }
        /// <summary>
        /// 弹出窗口显示 对象详细信息
        /// </summary>
        public class PopupObject : InspectorPlusAttribute
        {
            public float width = 300;
            public float heigth = 300;
            public string title = string.Empty;
        }
        /// <summary>
        /// 色板(未实现)
        /// </summary>
        public class ColorPalette : InspectorPlusAttribute
        {
            public string Name { get; private set; }
            public ColorPalette(string name)
            {
                this.Name = name;
            }
        }
        public class EnumToggleButtons : InspectorPlusAttribute
        {
        }
        /// <summary>
        /// 分割线
        /// </summary>
        public class Line : InspectorPlusAttribute
        {
            public enum LineType
            {
                Normal
            }
            public LineType m_type;
            public Line(LineType type = LineType.Normal)
            {
                m_type = type;
            }
        }

        /// <summary>
        /// 按钮
        /// </summary>
        [AttributeUsage(AttributeTargets.Method)]
        public class Button : Attribute
        {
            public bool Playing = true;
            public bool Editor;
            public string Title;
            public string Help;
        }

        public class TitleAttribute : InspectorPlusAttribute
        {
            public string Name { get; private set; }
            public TitleAttribute(string name)
            {
                this.Name = name;
            }
        }

        public class HorizontalGroupAttribute : InspectorPlusAttribute
        {
            public string Name { get; private set; }
            public HorizontalGroupAttribute(string name)
            {
                this.Name = name;
            }
        }
        /// <summary>
        /// 限制范围
        /// </summary>
        public class Range : InspectorPlusAttribute
        {
            public float Min { get; private set; }
            public float Max { get; private set; }
            public Range(float min, float max)
            {
                this.Min = min;
                this.Max = max;
            }
        }
        /// <summary>
        /// 最大值
        /// </summary>
        public class MaxValueAttribute : InspectorPlusAttribute
        {
            public float Value { get; private set; }
            public MaxValueAttribute(float value)
            {
                this.Value = value;
            }
        }
        /// <summary>
        /// 最小值
        /// </summary>
        public class MinValueAttribute : InspectorPlusAttribute
        {
            public float Value { get; private set; }
            public MinValueAttribute(float value)
            {
                this.Value = value;
            }
        }
        /// <summary>
        /// 参数变化时调用方法
        /// </summary>
        public class OnValueChangedAttribute : InspectorPlusAttribute
        {
            public string Name { get; private set; }
            public OnValueChangedAttribute(string name)
            {
                this.Name = name;
            }
        }
        /// <summary>
        /// scriptableobject引用直接显示
        /// </summary>
        //edit from https://forum.unity.com/threads/editor-tool-better-scriptableobject-inspector-editing.484393/#post-3519377
        public class ExpandableAttribute : InspectorPlusAttribute
        {
            public bool SetRuntime = true;
        }

    }
}
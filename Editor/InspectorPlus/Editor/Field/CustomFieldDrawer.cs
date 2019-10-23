using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this drawer used in no unity SerializedProperty field
/// eg dynamic field in runtime
/// </summary>
public class CustomFieldDrawer:Attribute
{
    public Type m_type;
    public CustomFieldDrawer(Type type)
    {
        this.m_type = type;
    }
}

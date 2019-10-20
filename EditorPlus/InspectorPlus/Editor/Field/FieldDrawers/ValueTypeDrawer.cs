using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CustomFieldDrawer(typeof(ValueType))]
public class ValueTypeDrawer : FieldDrawer
{
    public override object OnGUI(params GUILayoutOption[] options)
    {
       return FieldDrawerUtil.ValueTypeField(this.Title, (ValueType)this.instance  ,this.customAttribute);
    }
}

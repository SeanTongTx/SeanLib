using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct RangeFloat
{
    public RangeFloat(float start,float length)
    {
        this.start = start;
        this.length = length;
    }
    public float start;
    public float length;
    public float end { get { return start + length; } }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformHelper
{
    public static void SetParentLocalScale(this Transform source,Transform parent,bool worldPositionStays=false)
    {
        var localScale = source.localScale;
        source.SetParent(parent, worldPositionStays);
        source.localScale = localScale;
    }
    public static void SyncTransform(this Transform source,Transform destination)
    {
       // Matrix4x4 trs = Matrix4x4.TRS(destination.position, destination.rotation, destination.lossyScale);
        source.position = destination.position;
        source.rotation = destination.rotation;
      //  destination.localScale = trs.GetScale();
    }
    public static Quaternion GetRotation(this Matrix4x4 matrix)
    {
        float qw = Mathf.Sqrt(1f + matrix.m00+matrix.m11 + matrix.m22) / 2;
        float w = 4 * qw;
        float qx = (matrix.m21 - matrix.m12) / w;
        float qy = (matrix.m02 - matrix.m20) / w;
        float qz = (matrix.m10 - matrix.m01) / w;
        return new Quaternion(qx, qy, qz, qw);
    }
    public static Vector3 GetPostion(this Matrix4x4 matrix)
    {
        return new Vector3(matrix.m03, matrix.m13, matrix.m23);
    }
    public static Vector3 GetScale(this Matrix4x4 matrix)
    {
        var x = Mathf.Sqrt(matrix.m00 * matrix.m00 + matrix.m01 * matrix.m01 + matrix.m02 * matrix.m02);
        var y = Mathf.Sqrt(matrix.m10 * matrix.m10 + matrix.m11 * matrix.m11 + matrix.m12 * matrix.m12);
        var z = Mathf.Sqrt(matrix.m20 * matrix.m20 + matrix.m21 * matrix.m21 + matrix.m22 * matrix.m22);
        return new Vector3(x, y, z);
    }
}

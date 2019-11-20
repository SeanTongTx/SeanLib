using UnityEngine;

namespace SeanLib.Core
{
    public static class UnityValueHelper
    {
        /// <summary>
        /// 根据eulerAngles获得Quaternion
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Quaternion GetQuaternion(float x, float y, float z)
        {
            Quaternion q = new Quaternion();
            q.eulerAngles = new Vector3(x, y, z);
            return q;
        }
        /// <summary>
        /// 设置transform Rotation的x
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void SetRotationX(this Transform transform, float x)
        {
            Vector3 eulerAngles = transform.rotation.eulerAngles;
            transform.rotation = GetQuaternion(x, eulerAngles.y, eulerAngles.z);
        }
        /// <summary>
        /// 设置transform Rotation的y
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void SetRotationY(this Transform transform, float y)
        {
            Vector3 eulerAngles = transform.rotation.eulerAngles;
            transform.rotation = GetQuaternion(eulerAngles.x, y, eulerAngles.z);
        }
        /// <summary>
        /// 设置transform Rotation的z
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void SetRotationZ(this Transform transform, float z)
        {
            Vector3 eulerAngles = transform.rotation.eulerAngles;
            transform.rotation = GetQuaternion(eulerAngles.x, eulerAngles.y, z);
        }
        /// <summary>
        /// 设置transform Rotation
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void SetRotation(this Transform transform, float x,float y,float z)
        {
            transform.rotation = GetQuaternion(x, y, z);
        }
        /// <summary>
        /// 设置transform LocalPosition的x
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void SetLocalPositionX(this Transform transform, float x)
        {
            Vector3 v = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
            transform.localPosition = v;
        }
        /// <summary>
        /// 设置transform LocalPosition的y
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void SetLocalPositionY(this Transform transform, float y)
        {
            Vector3 v = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
            transform.localPosition = v;
        }
        /// <summary>
        /// 设置transform LocalPosition的z
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void SetLocalPositionZ(this Transform transform, float z)
        {
            Vector3 v = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
            transform.localPosition = v;
        }
        /// <summary>
        /// 设置transform Position的x
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void SetPositionX(this Transform transform, float x)
        {
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        /// <summary>
        /// 设置transform Position的y
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void SetPositionY(this Transform transform, float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
        /// <summary>
        /// 设置transform Position的z
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void SetPositionZ(this Transform transform, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }
        /// <summary>
        /// 获得朝向
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="lookTo"></param>
        /// <returns></returns>
        public static Vector3 GetLookToAngle(this Transform transform, Transform lookTo)
        {
            Quaternion localRotation = transform.localRotation;
            transform.LookAt(lookTo);
            Quaternion quaternion = transform.localRotation;
            transform.localRotation = localRotation;
            return quaternion.eulerAngles;
        }
        /// <summary>
        /// 获得朝向
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="lookTo"></param>
        /// <returns></returns>
        public static Vector3 GetLookToAngle(this Transform transform, Vector3 lookTo)
        {
            Quaternion localRotation = transform.localRotation;
            transform.LookAt(lookTo);
            Quaternion quaternion = transform.localRotation;
            transform.localRotation = localRotation;
            return quaternion.eulerAngles;
        }
        /// <summary>
        /// 获得一个修改alpha的眼设置
        /// </summary>
        /// <param name="color"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Color GetModifyAlpha(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }
        /// <summary>
        /// 把角度处理到-180~180之间
        /// </summary>
        /// <param name="v"></param>
        /// <param name="n180">保持负180，否则会变成180</param>
        /// <param name="p180">保持正180，否则会变成-180</param>
        /// <returns></returns>
        public static Vector3 IntoN180To180(this Vector3 v, bool n180 = true, bool p180 = true)
        {
            v.x %= 360;
            if ((v.x < -180) || (!n180 && v.x==-180)) v.x += 360;
            else if ((v.x > 180) || (!p180 && v.x == 180)) v.x -= 360;

            v.y %= 360;
            if ((v.y < -180) || (!n180 && v.y == -180)) v.y += 360;
            else if ((v.y > 180) || (!p180 && v.y == 180)) v.y -= 360;

            v.z %= 360;
            if ((v.z < -180) || (!n180 && v.z == -180)) v.z += 360;
            else if ((v.z > 180) || (!p180 && v.z == 180)) v.z -= 360;
            return v;
        }
        /// <summary>
        /// 把角度处理到-180~180之间，并且去接近一个值
        /// </summary>
        /// <param name="v"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static Vector3 IntoN180To180(this Vector3 v, Vector3 reference)
        {
            int ncount = (reference.x < 0 ? 1 : 0) + (reference.y < 0 ? 1 : 0) + (reference.z < 0 ? 1 : 0);
            int pcount = (reference.x > 0 ? 1 : 0) + (reference.y > 0 ? 1 : 0) + (reference.z > 0 ? 1 : 0);
            return v.IntoN180To180(ncount > pcount, pcount > ncount);
        }
    }
}

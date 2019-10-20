using UnityEngine;
using System.Collections;
namespace SeanLib.Core
{
    public static class RectHelper
    {
        public static Rect Rect100 = new Rect(0, 0, 100, 100);
        public static Rect Rect20 = new Rect(0, 0, 20, 20);
        public static Rect Rect0 = new Rect(0, 0, 0, 0);
        public static Vector2 Center(this Rect rect)
        {
            return rect.position + new Vector2(rect.width / 2, rect.height / 2);
        }
        public static Vector2 TopLeft(this Rect rect)
        {
            return rect.position;
        }
        public static Vector2 TopRight(this Rect rect)
        {
            return rect.position.DeltaXNew(rect.width);
        }
        public static Vector2 BottomLeft(this Rect rect)
        {
            return rect.position.DeltaYNew(rect.height);
        }
        public static Vector2 BottomRight(this Rect rect)
        {
            return rect.position + new Vector2(rect.width, rect.height);
        }

        public static Rect ToPostion(this Rect rect, Vector2 pos)
        {
            Rect newRect = new Rect(rect);
            newRect.position = pos;
            return newRect;
        }
        /// <summary>
        /// 均匀扩展 或收缩
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="sizeDelta"></param>
        /// <returns></returns>
        public static Rect SizeNew(this Rect rect, float sizeDelta)
        {
            Rect newRect = new Rect(rect.position.x - sizeDelta, rect.position.y - sizeDelta, rect.width + sizeDelta * 2, rect.height + sizeDelta * 2);
            return newRect;
        }
        public static Rect WidthNew(this Rect rect, float NewWidth)
        {
            Rect r = new Rect(rect);
            r.width = NewWidth;
            return r;
        }
        public static Rect HeightNew(this Rect rect, float NewHeight)
        {
            Rect r = new Rect(rect);
            r.height = NewHeight;
            return r;
        }
        public static Rect Delta(this Rect rect, Vector2 delta)
        {
            Rect newrect = new Rect(rect.position + delta, new Vector2(rect.width, rect.height));
            return newrect;
        }
    }
}
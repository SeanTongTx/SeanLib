using UnityEngine;
namespace SeanLib.Core
{
    public static class Vector2Helper
    {
        public static Vector2 DeltaX(this Vector2 vector2, float deltaX)
        {
            vector2.x = vector2.x + deltaX;
            return vector2;
        }
        public static Vector2 DeltaY(this Vector2 vector2, float deltaY)
        {
            vector2.y = vector2.y + deltaY;
            return vector2;
        }
        public static Vector2 DeltaXNew(this Vector2 vector2, float deltaX)
        {
            return new Vector2(vector2.x + deltaX, vector2.y);
        }
        public static Vector2 DeltaYNew(this Vector2 vector2, float deltaY)
        {
            return new Vector2(vector2.x, vector2.y + deltaY);
        }
    }
}
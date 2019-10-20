using UnityEngine;

namespace SeanLib.Core
{
    public static class BehaviourHelper
    {
        /// <summary>
        /// Behaviour是否处于活动状态
        /// </summary>
        /// <param name="mb">检测对象</param>
        /// <returns>是否处于活动状态</returns>
        static public bool IsActive(this Behaviour mb)
        {
            return mb && mb.enabled && mb.gameObject.activeInHierarchy;
        }
    }
}
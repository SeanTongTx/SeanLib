using UnityEngine;

/* *****************************************************************************
 * File:    UnityGoExtensions.cs
 * Author:  Philip Pierce - Friday, September 26, 2014
 * Description:
 *  Unity extensions on GameObjects
 *  
 * History:
 *  Friday, September 26, 2014 - Created
 * ****************************************************************************/

namespace SeanLib.Core
{
    /// <summary>
    /// Unity extensions on GameObjects
    /// </summary>
    public static class UnityGoExtensions
    {
        #region IsActive

        /// <summary>
        /// Returns true if the GO is not null and is active
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool IsActive(this GameObject go)
        {
            return ((go != null) && (go.activeSelf));
        }

        // IsActive

        #endregion
    }
}
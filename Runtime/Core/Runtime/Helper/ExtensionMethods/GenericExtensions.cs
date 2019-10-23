using System.Collections.Generic;
/* *****************************************************************************
     * File:    GenericExtensions.cs
     * Author:  Philip Pierce - Thursday, September 18, 2014
     * Description:
     *  Generic (T) Extensions
     *  
     * History:
     *  Thursday, September 18, 2014 - Created
     * ****************************************************************************/
namespace SeanLib.Core
{
    /// <summary>
    /// Generic (T) Extensions
    /// </summary>
    public static class GenericExtensions
    {

        #region IsTNull

        /// <summary>
        /// Returns true if the generic T is null or default. 
        /// This will match: null for classes; null (empty) for Nullable&lt;T&gt;; zero/false/etc for other structs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tObj"></param>
        /// <returns></returns>
        public static bool IsTNull<T>(this T tObj)
        {
            return (EqualityComparer<T>.Default.Equals(tObj, default(T)));
        }

        // IsTNull

        #endregion
    }
}
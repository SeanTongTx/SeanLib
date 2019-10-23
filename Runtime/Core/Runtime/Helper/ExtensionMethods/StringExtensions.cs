/* *****************************************************************************
 * File:    StringExtensions.cs
 * Author:  Philip Pierce - Tuesday, September 09, 2014
 * Description:
 *  Extensions for strings
 *  
 * History:
 *  Tuesday, September 09, 2014 - Created
 * ****************************************************************************/
namespace SeanLib.Core
{
    /// <summary>
    /// Extensions for strings
    /// </summary>
    public static class StringExtensions
    {
        #region IsNullOrEmpty

        /// <summary>
        /// Null or empty check as extension
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        // IsNullOrEmpty

        #endregion

        #region IsNOTNullOrEmpty

        /// <summary>
        /// Returns true if the string is Not null or empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNOTNullOrEmpty(this string value)
        {
            return (!string.IsNullOrEmpty(value));
        }

        // IsNOTNullOrEmpty

        #endregion

    }
}
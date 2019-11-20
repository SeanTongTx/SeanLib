using System.Collections.Generic;


namespace SeanLib.Core
{
    /// <summary>
    /// Dictionary Extensions
    /// </summary>
    public static class DictionaryExtensions
    {
        #region AddIfNotExists

        /// <summary>
        /// Method that adds the given key and value to the given dictionary only if the key is NOT present in the dictionary.
        /// This will be useful to avoid repetitive "if(!containskey()) then add" pattern of coding.
        /// </summary>
        /// <param name="dict">The given dictionary.</param>
        /// <param name="key">The given key.</param>
        /// <param name="value">The given value.</param>
        /// <returns>True if added successfully, false otherwise.</returns>
        /// <typeparam name="TKey">Refers the TKey type.</typeparam>
        /// <typeparam name="TValue">Refers the TValue type.</typeparam>
        public static bool AddIfNotExists<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
                return false;

            dict.Add(key, value);
            return true;
        }

        // AddIfNotExists

        #endregion

        #region AddOrReplace

        /// <summary>
        /// Method that adds the given key and value to the given dictionary if the key is NOT present in the dictionary.
        /// If present, the value will be replaced with the new value.
        /// </summary>
        /// <param name="dict">The given dictionary.</param>
        /// <param name="key">The given key.</param>
        /// <param name="value">The given value.</param>
        /// <typeparam name="TKey">Refers the Key type.</typeparam>
        /// <typeparam name="TValue">Refers the Value type.</typeparam>
        public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
                dict[key] = value;
            else
                dict.Add(key, value);
        }

        // AddOrReplace

        #endregion
    }
}

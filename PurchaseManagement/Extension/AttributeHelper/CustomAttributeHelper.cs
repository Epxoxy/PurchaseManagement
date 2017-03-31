using System;
using System.Collections.Generic;
using System.Linq;

namespace PurchaseManagement.Extension
{
    /// <summary>
    /// Custom attribute helper, easy to get attribute
    /// </summary>
    public static class CustomAttributeHelper
    {
        /// <summary>
        /// /// Cache Data
        /// /// </summary>
        private static readonly Dictionary<string, object> Cache = new Dictionary<string, object>();
        
        /// <summary>
        /// Get CustomAttribute Value
        /// </summary>
        /// <typeparam name="T">Attribute type</typeparam>
        /// <param name="sourceType">sourceType</param>
        /// <param name="attributeValueAction">attributeValueAction</param>
        /// <param name="name">field name or property name</param>
        /// <returns>Attribute object</returns>
        public static object GetCustomAttributeValue<T>(this Type sourceType, string name = null) where T : Attribute
        {
            return GetAttributeValue<T>(sourceType, name);
        }

        private static object GetAttributeValue<T>(Type sourceType, string name) where T : Attribute
        {
            var key = BuildKey(sourceType, name);
            if (!Cache.ContainsKey(key))
            {
                CacheAttributeValue<T>(key, sourceType, name);
            }

            return Cache[key];
        }

        /// <summary>
        /// Cache Attribute Object
        /// </summary>
        private static void CacheAttributeValue<T>(string key, Type type,string name)
        {
            var value = GetValue<T>(type, name);

            lock (key + "_attributeValueLockKey")
            {
                if (!Cache.ContainsKey(key))
                {
                    Cache[key] = value;
                }
            }
        }

        /// <summary>
        /// Get Attribute Object's Value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static object GetValue<T>(Type type, string name)
        {
            object attribute = default(T);
            if (string.IsNullOrEmpty(name))
            {
                attribute = type.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            }
            else
            {
                var propertyInfo = type.GetProperty(name);
                if (propertyInfo != null)
                {
                    attribute = propertyInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
                }

                var fieldInfo = type.GetField(name);
                if (fieldInfo != null)
                {
                    attribute = fieldInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
                }
            }

            return attribute;
        }

        /// <summary>
        /// Cache Collection Name Key
        /// </summary>
        private static string BuildKey(Type type, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return type.FullName;
            }

            return type.FullName + "." + name;
        }

    }
}

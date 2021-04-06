using System;
using UnityEngine;

namespace Itsdits.Ravar.Data
{
    /// <summary>
    /// JSON Helper that handles serializing and deserializing arrays of objects.
    /// </summary>
    public static class JsonHelper
    {
        // This class is from the following SO answer:
        // https://stackoverflow.com/a/36244111

        /// <summary>
        /// Deserializes a Json string into an array of an object type.
        /// </summary>
        /// <typeparam name="T">Object type of the array.</typeparam>
        /// <param name="json">Json string to deserialize.</param>
        /// <returns>Array of object type T containing the items in the Json string.</returns>
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        /// <summary>
        /// Serializes an array of objects into a Json string.
        /// </summary>
        /// <typeparam name="T">Object type of the array.</typeparam>
        /// <param name="array">Array of objects to be serialized.</param>
        /// <returns>Json string containing the items in the array of objects.</returns>
        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        /// <summary>
        /// Serializes an array of objects into a Json string.
        /// </summary>
        /// <typeparam name="T">Object type of the array.</typeparam>
        /// <param name="array">Array of objects to be serialized.</param>
        /// <param name="prettyPrint">Serializes the Json string with pretty print to make it more human readable.</param>
        /// <returns>Json string containing the items in the array of objects.</returns>
        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}

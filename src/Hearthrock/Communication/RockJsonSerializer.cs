// <copyright file="RockJsonSerializer.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Communication
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Facebook.MiniJSON;

    /// <summary>
    /// A JsonSerializer based on Facebook.MiniJSON.
    /// </summary>
    public class RockJsonSerializer
    {
        /// <summary>
        /// List of built-in types.
        /// </summary>
        private static readonly List<Type> BuiltinTypes = new List<Type>
        {
            typeof(byte),
            typeof(sbyte),
            typeof(char),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(bool),
            typeof(short),
            typeof(ushort),
            typeof(string)
        };

        /// <summary>
        /// Deserialize a json string to an object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="json">The json string.</param>
        /// <returns>The object deserialized.</returns>
        public static T Deserialize<T>(string json)
        {
            var obj = Json.Deserialize(json);
            return (T)ConvertToType(obj, typeof(T));
        }

        /// <summary>
        /// Serialize an object to a json string.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The serialized json string.</returns>
        public static string Serialize(object obj)
        {
            return Json.Serialize(ConvertToGenetal(obj, obj.GetType()));
        }

        /// <summary>
        /// Convert an object to a certain type.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="type">The type.</param>
        /// <returns>The object converted.</returns>
        private static object ConvertToType(object obj, Type type)
        {
            if (IsBuiltinType(type))
            {
                return Convert.ChangeType(obj, type);
            }

            if (type.IsEnum)
            {
                return Convert.ChangeType(obj, typeof(int));
            }

            object instance = Activator.CreateInstance(type);

            if (instance is IList && type.IsGenericType)
            {
                var list = instance as IList;
                Type genericArgument = type.GetGenericArguments()[0];
                var array = obj as List<object>;

                foreach (var item in array)
                {
                    list.Add(ConvertToType(item, genericArgument));
                }

                return list;
            }

            var map = obj as Dictionary<string, object>;

            if (instance is IDictionary && type.IsGenericType)
            {
                var dictionary = instance as IDictionary;
                Type genericArgument = type.GetGenericArguments()[1];

                foreach (var item in map)
                {
                    dictionary.Add(item.Key, ConvertToType(item.Value, genericArgument));
                }

                return dictionary;
            }

            // other types
            foreach (var pro in instance.GetType().GetProperties())
            {
                try
                {
                    pro.SetValue(instance, ConvertToType(map[pro.Name], pro.PropertyType), null);
                }
                catch
                {
                    // Ignore.
                }
            }

            return instance;
        }

        /// <summary>
        /// Convert an object to a general type.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="type">The type.</param>
        /// <returns>The object converted.</returns>
        private static object ConvertToGenetal(object obj, Type type)
        {
            foreach (var builtinTypes in BuiltinTypes)
            {
                if (ReferenceEquals(builtinTypes, type))
                {
                    if (obj == null)
                    {
                        if (type == typeof(string))
                        {
                            return string.Empty;
                        }

                        return Activator.CreateInstance(type);
                    }
                    else
                    {
                        return obj;
                    }
                }
            }

            if (type.IsEnum)
            {
                return (int)obj;
            }

            if (obj == null)
            {
                return null;
            }

            if (obj is IList && type.IsGenericType)
            {
                Type genericArgument = type.GetGenericArguments()[0];

                var token = new List<object>();
                var list = obj as IList;

                foreach (var item in list)
                {
                    token.Add(ConvertToGenetal(item, genericArgument));
                }

                return token;
            }
            else
            {
                var token = new Dictionary<string, object>();

                if (obj is IDictionary && type.IsGenericType)
                {
                    var dictionary = obj as IDictionary;
                    Type genericArgument = type.GetGenericArguments()[1];

                    foreach (var key in dictionary.Keys)
                    {
                        token.Add(key.ToString(), ConvertToGenetal(dictionary[key], genericArgument));
                    }

                    return token;
                }

                // other types
                foreach (var pro in obj.GetType().GetProperties())
                {
                    try
                    {
                        token.Add(pro.Name, ConvertToGenetal(pro.GetValue(obj, null), pro.PropertyType));
                    }
                    catch
                    {
                        // Ignore.
                    }
                }

                return token;
            }
        }

        /// <summary>
        /// Is built-in Type.
        /// Avoid to use Contains because Type equal might not be implemented.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type is build in type.</returns>
        private static bool IsBuiltinType(Type type)
        {
            foreach (var builtinTypes in BuiltinTypes)
            {
                if (ReferenceEquals(builtinTypes, type))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

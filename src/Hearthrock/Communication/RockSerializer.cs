// <copyright file="RockJsonSerializer.cs" company="https://github.com/yangyuan">
//     Copyright (c) The Hearthrock Project. All rights reserved.
// </copyright>

namespace Hearthrock.Communication
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Facebook.MiniJSON;

    public class RockJsonSerializer
    {
        public static T Deserialize<T>(string json)
        {
            var obj = Json.Deserialize(json);
            return (T)Construct(obj, typeof(T));
        }

        public static string Serialize(object obj)
        {
            return Json.Serialize(ToToken(obj, obj.GetType()));
        }

        private static object Construct(object obj, Type type)
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
                    list.Add(Construct(item, genericArgument));
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
                    dictionary.Add(item.Key, Construct(item.Value, genericArgument));
                }

                return dictionary;
            }

            // other types
            foreach (var pro in instance.GetType().GetProperties())
            {
                try
                {
                    pro.SetValue(instance, Construct(map[pro.Name], pro.PropertyType), null);
                }
                catch { }
            }

            return instance;
        }

        private static object ToToken(object obj, Type type)
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
                    token.Add(ToToken(item, genericArgument));
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
                        token.Add(key.ToString(), ToToken(dictionary[key], genericArgument));
                    }

                    return token;
                }

                // other types
                foreach (var pro in obj.GetType().GetProperties())
                {
                    token.Add(pro.Name, ToToken(pro.GetValue(obj, null), pro.PropertyType));
                }

                return token;
            }
        }

        private static bool IsBuiltinType (Type type)
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

        private static readonly List<Type> BuiltinTypes = new List<Type> {
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
            typeof(string) };
    }
}

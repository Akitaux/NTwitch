﻿using NTwitch.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTwitch.Chat
{
    internal partial class ChatParser
    {
        public static void PopulateObject(string msg, object obj, BaseRestClient client)
        {
            try
            {
                var split = msg.Split(new[] { ' ' }, 2);
                string data = split[0];
                string content = split[1];

                var properties = GetProperties<ChatPropertyAttribute>(obj);

                foreach (var p in properties)
                {
                    var attr = p.GetCustomAttribute<ChatPropertyAttribute>();

                    object value;
                    if (attr.Name == null)
                    {
                        value = Activator.CreateInstance(p.PropertyType, new[] { client });
                        PopulateObject(msg, value, client);
                    }
                    else
                    {
                        string name = attr.Name + "=";
                        string result = GetValueBetween(data, name, ";");

                        if (p.PropertyType == typeof(bool))
                            value = result == "1";
                        else
                            value = Convert.ChangeType(result, p.PropertyType);
                    }

                    if (value != null)
                        p.SetValue(obj, value);
                }

                var betweens = GetProperties<ChatValueBetweenAttribute>(obj);

                foreach (var b in betweens)
                {
                    var attr = b.GetCustomAttribute<ChatValueBetweenAttribute>();

                    string value = GetValueBetween(content, attr.FromValue, attr.ToValue);

                    if (value != null)
                        b.SetValue(obj, value);
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static string GetValueBetween(string data, string start, string end)
        {
            int valueStart = data.IndexOf(start) + start.Length;
            if (valueStart < 0)
                return null;

            int valueEnd;
            if (end == null)
                valueEnd = data.Substring(valueStart).Length - 1;
            else
                valueEnd = data.Substring(valueStart).IndexOf(end);

            string result = data.Substring(valueStart, valueEnd);
            return result;
        }

        public static IEnumerable<PropertyInfo> GetProperties<T>(object obj) where T : Attribute
        {
            var type = obj.GetType().GetTypeInfo();
            return type.GetProperties().Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(T)));
        }
    }
}

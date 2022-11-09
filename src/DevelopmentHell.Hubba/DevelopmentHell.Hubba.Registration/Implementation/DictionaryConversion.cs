﻿using System;
using System.Reflection;

public class DictionaryConversion
{
    public static Dictionary<string, object> ObjectToDictionary(object obj)
    {
        Dictionary<string, object> ret = new Dictionary<string, object>();

        foreach (PropertyInfo prop in obj.GetType().GetProperties())
        {
            string propName = prop.Name;
            var val = obj.GetType().GetProperty(propName)!.GetValue(obj, null);
            if (val != null)
            {
                ret.Add(propName, val);
            }
        }

        return ret;
    }
}
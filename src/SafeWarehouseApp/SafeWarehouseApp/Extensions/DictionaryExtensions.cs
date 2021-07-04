using System;
using System.Collections.Generic;

namespace SafeWarehouseApp.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue? TryGet<TValue>(this IDictionary<string, TValue> dictionary, string key, Func<TValue>? defaultValueProvider = default)
        {
            if (key == null!)
                return default;
            
            return !dictionary.ContainsKey(key) ? defaultValueProvider != null ? defaultValueProvider() : default : dictionary[key];
        }
    }
}
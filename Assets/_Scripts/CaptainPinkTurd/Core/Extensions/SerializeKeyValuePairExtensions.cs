using System.Collections.Generic;
using CaptainPinkTurd.Core.CustomDataStructure;
using CaptainPinkTurd.Core.Utilities;

namespace CaptainPinkTurd.Core.Extensions
{
    public static class SerializeKeyValuePairExtensions
    {
        //Work as a dictionary when used as array
        public static bool TryGetValue<TKey, TValue>(this SerializeKeyValuePair<TKey, TValue>[] dictionary, TKey key, out TValue value)
        {
            foreach (var element in dictionary)
            {
                if (EqualityComparer<TKey>.Default.Equals(element.Key, key))
                {
                    value = element.Value;
                    return true; 
                }
            }
            value = default;
            return false;
        }
    }
}
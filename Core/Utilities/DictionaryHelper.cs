using System.Collections.Generic;

namespace Utilities
{
    public static class DictionaryHelper
    {
        public static Dictionary<V, K> Reverse<K, V>(this IDictionary<K, V> sourceDictionary)
        {
            Dictionary<V, K> reverseDictionary = new Dictionary<V, K>();
            foreach (KeyValuePair<K, V> entry in sourceDictionary)
            {
                if (!reverseDictionary.ContainsKey(entry.Value))
                {
                    reverseDictionary.Add(entry.Value, entry.Key);
                }
            }
            return reverseDictionary;
        }
    }
}
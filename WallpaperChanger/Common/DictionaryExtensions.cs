using System.Collections.Generic;

namespace WallpaperChanger.Common
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> CopyToNew<TKey, TValue>(this Dictionary<TKey, TValue> source)
        {
            var result = new Dictionary<TKey, TValue>();

            foreach (var entry in source)
            {
                result.Add(entry.Key, entry.Value);
            }

            return result;
        }
    }
}

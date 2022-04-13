using System.Collections.Generic;

namespace ClocAnalyzerLibrary
{
    public static class ExtensionMethods
    {
        public static void IncrementBy<TKey>(this Dictionary<TKey, long> dictionary, TKey key, long valueToAdd)
        {
            dictionary.TryGetValue(key, out var count);
            dictionary[key] = count + valueToAdd;
        }

        public static void IncrementBy<TKey>(this Dictionary<TKey, (long, long, long, long)> dictionary, TKey key,
                                            long fileCount, long codeCount, long commentCount, long blankCount)
        {
            dictionary.TryGetValue(key, out (long, long, long, long) count);
            (long, long, long, long) newValue = (
                    count.Item1 + fileCount,
                    count.Item2 + codeCount,
                    count.Item3 + commentCount,
                    count.Item4 + blankCount);

            dictionary[key] = newValue;
        }
    }
}

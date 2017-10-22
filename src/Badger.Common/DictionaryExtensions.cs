using System;
using System.Collections.Generic;

namespace Badger.Common
{
    public static class DictionaryExtensions
    {
        public static Optional<TValue> Find<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out var value)) return Optional.Some(value);

            return Optional.None<TValue>();
        }

        public static Optional<TKey> FindKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, bool> predicate)
        {
            foreach (var kvp in dictionary)
            {
                if (predicate(kvp.Key, kvp.Value))
                {
                    return Optional.Some(kvp.Key);
                }
            }

            return Optional.None<TKey>();
        }

        public static Optional<U> Pick<TKey, TValue, U>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, Optional<U>> picker) 
        {
            foreach (var kvp in dictionary)
            {
                var pickResult = picker(kvp.Key, kvp.Value);
                if (pickResult.HasValue) return pickResult;
            }

            return Optional.None<U>();
        }
    }
}
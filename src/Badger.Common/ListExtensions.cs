using System;
using System.Collections.Generic;

namespace Badger.Common
{
    public static class ListExtensions
    {
        public static Optional<T> FindValue<T>(this IList<T> list, Func<T, bool> predicate) 
        {
            foreach (var item in list)
            {
                if (predicate(item)) return Optional.Some(item);
            }

            return Optional.None<T>();
        }

        public static Optional<U> Pick<T, U>(this IList<T> list, Func<T, Optional<U>> picker) 
        {
            foreach (var item in list)
            {
                var pickResult = picker(item);
                if (pickResult.HasValue) return pickResult;
            }

            return Optional.None<U>();
        }    
    }
}
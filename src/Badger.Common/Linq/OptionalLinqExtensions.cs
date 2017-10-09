using System;

namespace Badger.Common.Linq
{
    public static class OptionalLinqExtensions
    {
        public static Optional<U> Select<T, U>(this Optional<T> optional, Func<T, U> selector)
        {
            return optional.Map(selector);
        }

        public static Optional<U> SelectMany<T, U>(this Optional<T> optional, Func<T, Optional<U>> selector)
        {
            return optional.FlatMap(selector);
        }

        public static Optional<U2> SelectMany<T, U1, U2>(this Optional<T> optional, Func<T, Optional<U1>> flatMapper, Func<T, U1, U2> mapper)
        {
            return optional.FlatMap(flatMapper).Map(mapped => mapper(optional.Value, mapped));
        }

        public static Optional<T> Where<T>(this Optional<T> optional, Predicate<T> condition)
        {
            return optional.Filter(condition);
        }
    }
}
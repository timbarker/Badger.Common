using System;

namespace Badger.Common
{
    public static class OptionalLinqExtensions
    {
        public static Optional<TResult> Select<TSource, TResult>(this Optional<TSource> optional, Func<TSource, TResult> selector)
        {
            return optional.Map(selector);
        }

        public static Optional<TResult> SelectMany<TSource, TResult>(this Optional<TSource> optional, Func<TSource, Optional<TResult>> selector)
        {
            return optional.FlatMap(selector);
        }

        public static Optional<TResult> SelectMany<TSource, TOption, TResult>(this Optional<TSource> optional, Func<TSource, Optional<TOption>> optionalSelector, Func<TSource, TOption, TResult> resultSelector)
        {
            return optional.FlatMap(optionalSelector).Map(mapped => resultSelector(optional.Value, mapped));
        }

        public static Optional<T> Where<T>(this Optional<T> optional, Predicate<T> condition)
        {
            return optional.Filter(condition);
        }
    }
}
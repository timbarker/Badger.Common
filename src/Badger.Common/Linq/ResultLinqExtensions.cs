using System;

namespace Badger.Common.Linq
{
    public static class ResultLinqExtensions
    {
        public static Result<U, TError> Select<T, TError, U>(this Result<T, TError> result, Func<T, U> selector)
        {
            return result.Map(selector);
        }

        public static Result<U, TError> SelectMany<T, TError, U>(this Result<T, TError> result, Func<T, Result<U, TError>> selector)
        {
            return result.FlatMap(selector);
        }

        public static Result<U2, TError> SelectMany<T, TError, U1, U2>(this Result<T, TError> result, Func<T, Result<U1, TError>> flatMapper, Func<T, U1, U2> mapper)
        {
            return result.FlatMap(flatMapper).Map(mapped => mapper(result.Value, mapped));
        }
    }
}
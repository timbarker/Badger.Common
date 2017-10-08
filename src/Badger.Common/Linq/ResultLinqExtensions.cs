using System;

namespace Badger.Common.Linq
{
    public static class ResultLinqExtensions
    {
        public static Result<TSelectSuccess, TError> Select<TSuccess, TError, TSelectSuccess>(this Result<TSuccess, TError> result, Func<TSuccess, TSelectSuccess> selector)
        {
            return result.Map(selector);
        }

        public static Result<TMappedSuccess, TError> SelectMany<TMappedSuccess, TSuccess, TError>(this Result<TSuccess, TError> result, Func<TSuccess, Result<TMappedSuccess, TError>> selector)
        {
            return result.FlatMap(selector);
        }

        public static Result<TResult, TError> SelectMany<TMappedSuccess, TSuccess, TError, TResult>(this Result<TSuccess, TError> result, Func<TSuccess, Result<TMappedSuccess, TError>> optionalSelector, Func<TSuccess, TMappedSuccess, TResult> resultSelector)
        {
            return result.FlatMap(optionalSelector).Map(mapped => resultSelector(result.Success, mapped));
        }
    }
}
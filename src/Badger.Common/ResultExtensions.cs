using System;

namespace Badger.Common
{
    public static class ResultExtensions
    {
        public static Result<TMapSuccess, TError> FlatMap<TSuccess, TError, TMapSuccess>(this Result<TSuccess, TError> result, Func<TSuccess, Result<TMapSuccess, TError>> mapper)
        {
            return result.IsSuccess ? mapper(result.Success) : Result.Error<TMapSuccess, TError>(result.Error);
        }

        public static Result<TMapSuccess, TError> Map<TSuccess, TError, TMapSuccess>(this Result<TSuccess, TError> result, Func<TSuccess,TMapSuccess> mapper) 
        {
            return result.FlatMap(r => Result.Ok<TMapSuccess, TError>(mapper(r)));
        }

        public static Result<TSuccess, TMapError> MapError<TSuccess, TError, TMapError>(this Result<TSuccess, TError> result, Func<TError, TMapError> mapper)
        {
            return result.IsError ? Result.Error<TSuccess, TMapError>(mapper(result.Error)) : Result.Ok<TSuccess, TMapError>(result.Success);
        } 

        public static TSuccess SuccessOrThrow<TSuccess, TError>(this Result<TSuccess, TError> result) where TError : Exception
        {
            if (result.IsError) throw result.Error;

            return result.Success;
        }
    }
}
using System;
using System.Collections.Generic;

namespace Badger.Common
{
    public static class ResultExtensions
    {
        public static Result<U, TError> FlatMap<T, TError, U>(this Result<T, TError> result, Func<T, Result<U, TError>> mapper)
        {
            return result.HasValue ? mapper(result.Value) : Result.Error<U, TError>(result.Error);
        }

        public static Result<U, TError> Map<T, TError, U>(this Result<T, TError> result, Func<T, U> mapper)
        {
            return result.FlatMap(r => Result.Ok<U, TError>(mapper(r)));
        }

        public static Result<U, TError> Apply<T, TError, U>(this Result<T, TError> result, Result<Func<T, U>, TError> applier)
        {
            if (result.HasValue && applier.HasValue)
                return Result.Ok<U, TError>(applier.Value(result.Value));

            if (!applier.HasValue)
                return Result.Error<U, TError>(applier.Error);

            return Result.Error<U, TError>(result.Error);
        }

        public static Result<T, UError> MapError<T, TError, UError>(this Result<T, TError> result, Func<TError, UError> mapper)
        {
            return !result.HasValue ? Result.Error<T, UError>(mapper(result.Error)) : Result.Ok<T, UError>(result.Value);
        }

        public static Result<T, TError> WhenOk<T, TError>(this Result<T, TError> result, Action<T> action)
        {
            if (result.HasValue) action(result.Value);

            return result;
        }

        public static Result<T, TError> WhenError<T, TError>(this Result<T, TError> result, Action<TError> action)
        {
            if (!result.HasValue) action(result.Error);

            return result;
        }

        public static R Match<T, TError, R>(this Result<T, TError> result, Func<T, R> ok, Func<TError, R> error)
        {
            if (result.HasValue) return ok(result.Value);

            return error(result.Error);
        }

        public static T ValueOrThrow<T, TError>(this Result<T, TError> result) where TError : Exception
        {
            if (!result.HasValue) throw result.Error;

            return result.Value;
        }

        public static T ValueOr<T, TError>(this Result<T, TError> result, T @default)
        {
            if (result.HasValue) return result.Value;
            return @default;
        }

        public static T ValueOr<T, TError>(this Result<T, TError> result, Func<T> getDefault)
        {
            if (result.HasValue) return result.Value;
            return getDefault();
        }

        public static T ValueOr<T, TError>(this Result<T, TError> result, Func<TError, T> getDefault)
        {
            if (result.HasValue) return result.Value;
            return getDefault(result.Error);
        }

        public static IEnumerable<T> AsEnumerable<T, TError>(this Result<T, TError> result)
        {
            if (result.HasValue) yield return result.Value;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Badger.Common
{
    public static class ValidationExtensions
    {
        public static Validation<U, TError> Apply<T, TError, U>(this Validation<T, TError> validation, Validation<Func<T, U>, TError> applier)
        {
            if (validation.Success && applier.Success)
                return Validation.Success<U, TError>(applier.Value(validation.Value));

            if (!validation.Success && applier.Success)
                return Validation.Error<U, TError>(validation.Errors);

            if (validation.Success && !applier.Success)
                return Validation.Error<U, TError>(applier.Errors);

            return Validation.Error<U, TError>(validation.Errors.Concat(applier.Errors));
        }

        public static Validation<U, TError> FlatMap<T, TError, U>(this Validation<T, TError> validation, Func<T, Validation<U, TError>> mapper)
        {
            return validation.Success ? mapper(validation.Value) : Validation.Error<U, TError>(validation.Errors.ToArray());
        }

        public static Validation<U, TError> Map<T, TError, U>(this Validation<T, TError> validation, Func<T, U> mapper)
        {
            return validation.FlatMap(r => Validation.Success<U, TError>(mapper(r)));
        }

        public static Validation<T, UError> MapError<T, TError, UError>(this Validation<T, TError> validation, Func<TError, UError> mapper)
        {
            return !validation.Success ? Validation.Error<T, UError>(validation.Errors.Select(mapper)) : Validation.Success<T, UError>(validation.Value);
        }

        public static Validation<T, TError> WhenSuccess<T, TError>(this Validation<T, TError> validation, Action<T> whenSuccess)
        {
            if (validation.Success) whenSuccess(validation.Value);

            return validation;
        }

        public static Validation<T, TError> WhenError<T, TError>(this Validation<T, TError> validation, Action<IEnumerable<TError>> whenError)
        {
            if (!validation.Success) whenError(validation.Errors);

            return validation;
        }

        public static R Match<T, TError, R>(this Validation<T, TError> validation, Func<T, R> success, Func<IEnumerable<TError>, R> error)
        {
            if (validation.Success) return success(validation.Value);

            return error(validation.Errors);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Badger.Common
{
    public abstract class Validation<T, TError>
    {
        internal Validation() { }

        public abstract bool Success { get; }

        internal abstract T Value { get; }

        internal abstract IEnumerable<TError> Errors { get; }
    }

    public static class Validation
    {
        private class SuccessValidation<T, TError> : Validation<T, TError>
        {
            public SuccessValidation(T value)
            {
                this.Value = value;
            }

            public override bool Success => true;

            internal override T Value { get; }

            internal override IEnumerable<TError> Errors => Enumerable.Empty<TError>();
        }

        private class ErrorValidation<T, TError> : Validation<T, TError>
        {
            public ErrorValidation(IEnumerable<TError> errors)
            {
                Errors = errors.ToArray();
            }

            public override bool Success => false;

            internal override T Value => throw new InvalidOperationException("Value can't be read on an error");

            internal override IEnumerable<TError> Errors { get; }
        }

        public static Validation<T, TError> Success<T, TError>(T ok)
        {
            return new SuccessValidation<T, TError>(ok);
        }

        public static Validation<T, TError> Error<T, TError>(IEnumerable<TError> errors)
        {
            return new ErrorValidation<T, TError>(errors);
        }

        public static Validation<T, TError> Error<T, TError>(TError error)
        {
            return new ErrorValidation<T, TError>(new[] { error });
        }
    }
}
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
        private class SuccessValidator<T, TError> : Validation<T, TError>
        {
            public SuccessValidator(T value)
            {
                this.Value = value;
            }

            public override bool Success => true;

            internal override T Value { get; }

            internal override IEnumerable<TError> Errors => Enumerable.Empty<TError>();
        }

        private class ErrorValidator<T, TError> : Validation<T, TError>
        {
            public ErrorValidator(IEnumerable<TError> errors)
            {
                Errors = errors.ToArray();
            }

            public override bool Success => false;

            internal override T Value => throw new InvalidOperationException();

            internal override IEnumerable<TError> Errors { get; }
        }

        public static Validation<T, TError> Success<T, TError>(T ok)
        {
            return new SuccessValidator<T, TError>(ok);
        }

        public static Validation<T, TError> Error<T, TError>(IEnumerable<TError> errors)
        {
            return new ErrorValidator<T, TError>(errors);
        }

        public static Validation<T, TError> Error<T, TError>(TError error)
        {
            return new ErrorValidator<T, TError>(new[] { error });
        }
    }
}
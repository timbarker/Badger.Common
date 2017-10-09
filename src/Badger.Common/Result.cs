using System;

namespace Badger.Common
{
    public abstract class Result<T, TError>
    {
        internal Result() {} 

        public abstract bool HasValue { get; }
        internal abstract T Value { get; }
        internal abstract TError Error { get; }     
    }

    public static class Result
    {
        public static Result<T, TError> Ok<T, TError>(T ok)
        {
            return new OkResult<T, TError>(ok);
        }

        public static Result<T, TError> Error<T, TError>(TError error)
        {
            return new ErrorResult<T, TError>(error);
        }

        public static Result<T, TError> Try<T, TError> (Func<T> work) where TError : Exception
        {
            try 
            {
                return Ok<T, TError>(work());
            }
            catch (TError ex)
            {
                return Error<T, TError>(ex);
            }
        }

        public static Result<T, Exception> Try<T>(Func<T> work)
        {
            return Try<T, Exception>(work);
        }

        private sealed class OkResult<T, TError> : Result<T, TError>
        {
            public OkResult(T ok)
            {
                Value = ok;
            }

            public override bool HasValue => true;
            internal override T Value { get; }
            internal override TError Error => throw new InvalidOperationException("Error value not available on Ok");

            public override string ToString()
            {
                return $"Ok{Value})";
            }
        }

        private sealed class ErrorResult<T, TError> : Result<T, TError>
        {
            public ErrorResult(TError error)
            {
                Error = error;
            }

            public override bool HasValue => false;
            internal override T Value => throw new InvalidOperationException("Ok value not available on Erorr");
            internal override TError Error { get; }

            public override string ToString()
            {
                return $"Error{Error})";
            }
        }
    }
}
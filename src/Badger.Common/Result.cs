using System;

namespace Badger.Common
{
    public abstract class Result<TSuccess, TError>
    {
        internal Result() {} 

        public abstract bool IsSuccess { get; }
        public abstract bool IsError { get; }
        internal abstract TSuccess Success { get; }
        internal abstract TError Error { get; }     
    }

    public static class Result
    {
        public static Result<TSuccess, TError> Ok<TSuccess, TError>(TSuccess success)
        {
            return new SuccessResult<TSuccess, TError>(success);
        }

        public static Result<TSuccess, TError> Error<TSuccess, TError>(TError error)
        {
            return new ErrorResult<TSuccess, TError>(error);
        }

        public static Result<TSuccess, TError> Try<TSuccess, TError> (Func<TSuccess> work) where TError : Exception
        {
            try 
            {
                return Ok<TSuccess, TError>(work());
            }
            catch (TError ex)
            {
                return Error<TSuccess, TError>(ex);
            }
        }

        public static Result<TSuccess, Exception> Try<TSuccess>(Func<TSuccess> work)
        {
            return Try<TSuccess, Exception>(work);
        }

        private sealed class SuccessResult<TSuccess, TError> : Result<TSuccess, TError>
        {
            public SuccessResult(TSuccess success)
            {
                Success = success;
            }

            public override bool IsSuccess => true;
            public override bool IsError => false;
            internal override TSuccess Success { get; }
            internal override TError Error => throw new InvalidOperationException("Error value not available on Success");

            public override string ToString()
            {
                return $"Success{Success})";
            }
        }

        private sealed class ErrorResult<TSuccess, TError> : Result<TSuccess, TError>
        {
            public ErrorResult(TError error)
            {
                Error = error;
            }

            public override bool IsSuccess => false;
            public override bool IsError => true;
            internal override TSuccess Success => throw new InvalidOperationException("Success value not available on Erorr");
            internal override TError Error { get; }

            public override string ToString()
            {
                return $"Error{Error})";
            }
        }
    }
}
using System;

namespace Badger.Common
{
    public abstract class Optional<T>
    {
        public abstract T Value { get; }

        public abstract bool HasValue { get; }
    }

    public static class Optional
    {
        public static Optional<T> Some<T>(T value)
        {
            return value != null
                ? new SomeOptional<T>(value)
                : throw new ArgumentNullException(nameof(value), "Some values must not be null");
        }

        public static Optional<T> None<T>()
        {
            return new NoneOptional<T>();
        }

        public static Optional<T> FromNullable<T>(T? value) where T : struct
        {
            if (value.HasValue) return Some(value.Value);
            return None<T>();
        }

        private sealed class SomeOptional<T> : Optional<T>
        {
            public SomeOptional(T value)
            {
                Value = value;
            }

            public override T Value { get; }

            public override bool HasValue => true;

            public override string ToString()
            {
                return $"Some({Value})";
            }
        }

        private sealed class NoneOptional<T> : Optional<T>
        {
            public override T Value => throw new InvalidOperationException("None does not have a value");

            public override bool HasValue => false;
        }
    }
}
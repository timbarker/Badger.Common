using System;

namespace Badger.Common
{
    public struct Optional<T>
    {
        private readonly T value;

        public Optional(T value)
        {
            this.value = value;
            HasValue = value != null;
        }

        public T Value
        {
            get
            {
                return HasValue ? value : throw new InvalidOperationException("Optional does not have a value");
            }
        }

        public bool HasValue { get; }

        public override string ToString()
        {
            return HasValue ? $"Optional({Value})" : "Optional()";
        }
    }

    public static class Optional
    {
        public static Optional<T> Some<T>(T value)
        {
            return value != null
                ? new Optional<T>(value)
                : throw new ArgumentNullException(nameof(value), "Some values must not be null");
        }

        public static Optional<T> None<T>()
        {
            return new Optional<T>();
        }

        public static Optional<T> FromNullable<T>(T? value) where T : struct
        {
            if (value.HasValue) return Some(value.Value);
            return None<T>();
        }
    }
}
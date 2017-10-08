using System;

namespace Badger.Common
{
    public static class OptionalExtensions
    {
        public static T ValueOr<T>(this Optional<T> optional, T @default)
        {
            if (optional.HasValue) return optional.Value;
            return @default;
        }

        public static T ValueOr<T>(this Optional<T> optional, Func<T> get)
        {
            if (optional.HasValue) return optional.Value;
            return get();
        }

        public static Optional<U> FlatMap<T, U>(this Optional<T> optional, Func<T, Optional<U>> mapper)
        {
            return optional.HasValue ? mapper(optional.Value) : Optional.None<U>();
        }

        public static Optional<U> Map<T, U>(this Optional<T> optional, Func<T, U> mapper)
        {
            return optional.FlatMap(t => Optional.Some(mapper(t)));
        }

        public static Optional<T> Filter<T>(this Optional<T> optional, Predicate<T> filter)
        {
            return optional.FlatMap(t => filter(t) ? Optional.Some(t) : Optional.None<T>());
        }

        public static T? ToNullable<T>(this Optional<T> optional) where T : struct
        {
            if (optional.HasValue) return optional.Value;
            else return null;
        }
    }
}
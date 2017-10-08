using Xunit;
using FluentAssertions;
using System;

namespace Badger.Common.Tests
{
    public class GivenANonNullNullable
    {
        public class WhenConvertingToAnOptional
        {
            private readonly Optional<int> optional;

            public WhenConvertingToAnOptional()
            {
                optional = Optional.FromNullable(new Nullable<int>(42));
            }

            [Fact]
            public void ThenTheValueIsCorrect()
            {
                optional.Value.Should().Be(42);
            }

            [Fact]
            public void ThenTheOptionalHasAValue()
            {
                optional.HasValue.Should().BeTrue();
            }
        }
    }

    public class GivenANullNullable
    {
        public class WhenConvertingToAnOptional
        {
            private readonly Optional<int> optional;

            public WhenConvertingToAnOptional()
            {
                optional = Optional.FromNullable(new Nullable<int>());
            }

            [Fact]
            public void ThenTheOptionalShouldNotHaveAValue()
            {
                optional.HasValue.Should().BeFalse();
            }
        }
    }

    public class GivenASome
    {
        public class WhenCreatingWithANullReference
        {
            [Fact]
            public void ThenAnNullReferenceExceptionIsThrown()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => Optional.Some<string>(null));
                ex.Message.Should().StartWith("Some values must not be null");
            }
        }

        public class WhenCreatingWithANullValue
        {
            [Fact]
            public void ThenAnNullReferenceExceptionIsThrown()
            {
                var ex = Assert.Throws<ArgumentNullException>(() => Optional.Some<int?>(null));
                ex.Message.Should().StartWith("Some values must not be null");
            }
        }

        public class WhenGettingTheValue
        {
            private readonly Optional<int> optional;

            public WhenGettingTheValue()
            {
                optional = Optional.Some(42);
            }

            [Fact]
            public void ThenTheValueShouldBeCorrect()
            {
                optional.Value.Should().Be(42);
            }

            [Fact]
            public void ThenTheValueShouldBePresent()
            {
                optional.HasValue.Should().BeTrue();
            }
        }

        public class WhenFlatMapping
        {
            private readonly Optional<int> result;

            public WhenFlatMapping()
            {
                result = Optional.Some(42).FlatMap(v => Optional.Some(v * 2));
            }

            [Fact]
            public void ThenTheFlatMapResultIsCorrect()
            {
                result.Value.Should().Be(84);
            }
        }

        public class WhenFlatMappingToNone
        {
            private readonly Optional<int> result;

            public WhenFlatMappingToNone()
            {
                result = Optional.Some(42).FlatMap(v => Optional.None<int>());
            }

            [Fact]
            public void ThenTheFlatMapResultIsCorrect()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenMapping
        {
            private readonly Optional<int> result;

            public WhenMapping()
            {
                result = Optional.Some(42).Map(v => v * 2);
            }

            [Fact]
            public void ThenTheMapResultIsCorrect()
            {
                result.Value.Should().Be(84);
            }
        }

        public class WhenFilteringIn
        {
            private readonly Optional<int> result;

            public WhenFilteringIn()
            {
                result = Optional.Some(42).Filter(v => v == 42);
            }

            [Fact]
            public void ThenTheFilterResultShouldBeCorrect()
            {
                result.Value.Should().Be(42);
            }
        }

        public class WhenFilteringOut
        {
            private readonly Optional<int> result;

            public WhenFilteringOut()
            {
                result = Optional.Some(42).Filter(v => v != 42);
            }

            [Fact]
            public void ThenTheFilterResultShouldBeCorrect()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenInvokingWhenSome
        {
            private readonly Optional<int> optional;

            public WhenInvokingWhenSome()
            {
                optional = Optional.Some(42);
            }

            [Fact]
            public void ThenTheActionIsInvoked()
            {
                int invokedArg = 0;
                optional.WhenSome(v => invokedArg = v);

                invokedArg.Should().Be(42);
            }

            [Fact]
            public void TheReturnedOptionShouldBeTheSame()
            {
                optional.WhenSome(_ => {}).Should().BeSameAs(optional);
            }
        }

        public class WhenInvokingWhenNone
        {
            private readonly Optional<int> optional;

            public WhenInvokingWhenNone()
            {
                optional = Optional.Some(42);
            }

            [Fact]
            public void ThenTheActionIsNotInvoked()
            {
                bool invoked = false;
                optional.WhenNone(() => invoked = true);

                invoked.Should().BeFalse();
            }

            [Fact]
            public void TheReturnedOptionShouldBeTheSame()
            {
                optional.WhenNone(() => {}).Should().BeSameAs(optional);
            }
        }

        public class WhenConvertingToNullable
        {
            private readonly int? result;

            public WhenConvertingToNullable()
            {
                result = Optional.Some(42).ToNullable();
            }

            [Fact]
            public void ThenTheNullableIsCorrect()
            {
                result.Should().Be(42);
            }
        }

        public class WhenConvertingToAValue
        {
            private readonly Optional<int> optional;

            public WhenConvertingToAValue()
            {
                optional = Optional.Some(42);
            }

            [Fact]
            public void ThenTheDefaultValueIsNotReturned()
            {
                optional.ValueOr(100).Should().Be(42);
            }

            [Fact]
            public void ThenTheDefaultValueProviderIsNotUsed()
            {
                optional.ValueOr(() => 100).Should().Be(42);
            }
        }

        public class WhenUsedInALinqExpression
        {
            private readonly Optional<int> optional;

            public WhenUsedInALinqExpression()
            {
                optional = Optional.Some(42);
            }

            private static Optional<int> Divide(int a, int b)
            {
                if (b == 0) return Optional.None<int>();
                return Optional.Some(a / b);
            }

            [Fact]
            public void ThenSelectWorksAsExpected()
            {
                var result = from v in optional
                             select v;

                result.Value.Should().Be(42);

                result = from v in optional
                         select v * 2;

                result.Value.Should().Be(84);
            }

            [Fact]
            public void ThenSelectManyWorksAsExpected()
            {
                var result = from v1 in optional
                             from v2 in Divide(420, v1)
                             select v2;

                result.Value.Should().Be(10);

                result = from v1 in optional
                         from v2 in Divide(v1, 0)
                         select v2;

                result.HasValue.Should().BeFalse();
            }

            [Fact]
            public void ThenWhereWorksAsExpected()
            {
                var result = from v in optional
                             where v == 42
                             select v;

                result.Value.Should().Be(42);

                result = from v in optional
                         where v != 42
                         select v;

                result.HasValue.Should().BeFalse();
            }
        }
    }

    public class GivenANone
    {
        public class WhenGettingTheValue
        {
            private readonly Optional<int> optional;

            public WhenGettingTheValue()
            {
                optional = Optional.None<int>();
            }

            [Fact]
            public void ThenTheValueShouldBeCorrect()
            {
                var ex = Assert.Throws<InvalidOperationException>(() => optional.Value);
                ex.Message.Should().Be("None does not have a value");
            }

            [Fact]
            public void ThenTheValueShouldBePresent()
            {
                optional.HasValue.Should().BeFalse();
            }
        }

        public class WhenFlatMapping
        {
            private readonly Optional<int> result;

            public WhenFlatMapping()
            {
                result = Optional.None<int>().FlatMap(v => Optional.Some(v * 2));
            }

            [Fact]
            public void ThenTheFlatMapResultIsCorrect()
            {
                result.HasValue.Should().BeFalse();
            }
        }


        public class WhenFlatMappingToNone
        {
            private readonly Optional<int> result;

            public WhenFlatMappingToNone()
            {
                result = Optional.None<int>().FlatMap(v => Optional.None<int>());
            }

            [Fact]
            public void ThenTheFlatMapResultIsCorrect()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenMapping
        {
            private readonly Optional<int> result;

            public WhenMapping()
            {
                result = Optional.None<int>().Map(v => v * 2);
            }

            [Fact]
            public void ThenTheMapResultIsCorrect()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenFilteringIn
        {
            private readonly Optional<int> result;

            public WhenFilteringIn()
            {
                result = Optional.None<int>().Filter(v => v == 42);
            }

            [Fact]
            public void ThenTheFilterResultShouldBeCorrect()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenFilteringOut
        {
            private readonly Optional<int> result;

            public WhenFilteringOut()
            {
                result = Optional.None<int>().Filter(v => v != 42);
            }

            [Fact]
            public void ThenTheFilterResultShouldBeCorrect()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenInvokingWhenSome
        {
            private readonly Optional<int> optional;

            public WhenInvokingWhenSome()
            {
                optional = Optional.None<int>();
            }

            [Fact]
            public void ThenTheActionIsNotInvoked()
            {
                bool invoked = false;
                optional.WhenSome(v => invoked = true);

                invoked.Should().BeFalse();
            }

            [Fact]
            public void TheReturnedOptionShouldBeTheSame()
            {
                optional.WhenSome(_ => {}).Should().BeSameAs(optional);
            }
        }

        public class WhenInvokingWhenNone
        {
            private readonly Optional<int> optional;

            public WhenInvokingWhenNone()
            {
                optional = Optional.None<int>();
            }

            [Fact]
            public void ThenTheActionIsInvoked()
            {
                bool invoked = false;
                optional.WhenNone(() => invoked = true);

                invoked.Should().BeTrue();
            }

            [Fact]
            public void TheReturnedOptionShouldBeTheSame()
            {
                optional.WhenNone(() => {}).Should().BeSameAs(optional);
            }
        }

        public class WhenConvertingToNullable
        {
            private readonly int? result;

            public WhenConvertingToNullable()
            {
                result = Optional.None<int>().ToNullable();
            }

            [Fact]
            public void ThenTheNullableIsCorrect()
            {
                result.Should().BeNull();
            }
        }

        public class WhenConvertingToAValue
        {
            private readonly Optional<int> optional;

            public WhenConvertingToAValue()
            {
                optional = Optional.None<int>();
            }

            [Fact]
            public void ThenTheDefaultValueIsReturned()
            {
                optional.ValueOr(100).Should().Be(100);
            }

            [Fact]
            public void ThenTheDefaultValueProviderIsUsed()
            {
                optional.ValueOr(() => 100).Should().Be(100);
            }
        }

        public class WhenUsedInALinqExpression
        {
            private readonly Optional<int> optional;

            public WhenUsedInALinqExpression()
            {
                optional = Optional.None<int>();
            }

            private static Optional<int> Divide(int a, int b)
            {
                if (b == 0) return Optional.None<int>();
                return Optional.Some(a / b);
            }

            [Fact]
            public void ThenSelectWorksAsExpected()
            {
                var result = from v in optional
                             select v;

                result.HasValue.Should().BeFalse();

                result = from v in optional
                         select v * 2;

                result.HasValue.Should().BeFalse();
            }

            [Fact]
            public void ThenSelectManyWorksAsExpected()
            {
                var result = from v1 in optional
                             from v2 in Divide(420, v1)
                             select v2;

                result.HasValue.Should().BeFalse();
                
                result = from v1 in optional
                         from v2 in Divide(v1, 0)
                         select v2;

                result.HasValue.Should().BeFalse();
            }

            [Fact]
            public void ThenWhereWorksAsExpected()
            {
                var result = from v in optional
                             where v == 42
                             select v;

                result.HasValue.Should().BeFalse();

                result = from v in optional
                         where v != 42
                         select v;

                result.HasValue.Should().BeFalse();
            }
        }
    }
}
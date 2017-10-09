using System;
using FluentAssertions;
using Xunit;
using Badger.Common.Linq;

namespace Badger.Common.Tests
{
    static class ResultTestHelperExtensions
    {
        public static void AssertOk<T, TError>(this Result<T, TError> result, T ok)
        {
            result
                .WhenOk(s => s.Should().Be(ok))
                .WhenError(e => Xunit.Assert.True(false, $"expected Ok: {ok} but got error: {e}"));
        }

        public static void AssertError<T, TError>(this Result<T, TError> result, TError error)
        {
            result
                .WhenError(e => e.Should().Be(error))
                .WhenOk(s => Xunit.Assert.True(false, $"expected error: {error} but got Ok: {s}"));
        }

        public static void AssertError<T, TError>(this Result<T, TError> result, Predicate<TError> errorCheck)
        {
            result
               .WhenError(e => errorCheck(e).Should().BeTrue())
               .WhenOk(s => Xunit.Assert.True(false, $"expected error but got Ok: {s}"));
        }
    }

    public class GivenAnOk
    {
        public class WhenResultOk
        {
            private readonly Result<int, string> result;

            public WhenResultOk()
            {
                result = Result.Ok<int, string>(42);
            }

            [Fact]
            public void ThenTheResultShouldHaveAValue()
            {
                result.HasValue.Should().BeTrue();
            }
        }

        public class WhenFlatMapping
        {
            private readonly Result<int, string> result;

            public WhenFlatMapping()
            {
                result = Result.Ok<int, string>(42).FlatMap(r => Result.Ok<int, string>(r * 2));
            }

            [Fact]
            public void ThenTheFlatMapResultIsCorrect()
            {
                result.AssertOk(84);
            }

            [Fact]
            public void ThenTheResultShouldHaveAValue()
            {
                result.HasValue.Should().BeTrue();
            }
        }

        public class WhenFlatMappingToError
        {
            private readonly Result<int, string> result;

            public WhenFlatMappingToError()
            {
                result = Result.Ok<int, string>(42).FlatMap(r => Result.Error<int, string>("Borked"));
            }

            [Fact]
            public void ThenTheFlatMapResultIsCorrect()
            {
                result.AssertError("Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotHaveAValue()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenMapping
        {
            private readonly Result<int, string> result;

            public WhenMapping()
            {
                result = Result.Ok<int, string>(42).Map(r => r * 2);
            }

            [Fact]
            public void ThenTheMapResultIsCorrect()
            {
                result.AssertOk(84);
            }

            [Fact]
            public void ThenTheResultShouldHaveAValue()
            {
                result.HasValue.Should().BeTrue();
            }
        }

        public class WhenMappingError
        {
            private readonly Result<int, string> result;

            public WhenMappingError()
            {
                result = Result.Ok<int, string>(42).MapError(e => "More " + e);
            }

            [Fact]
            public void ThenTheErrorMapResultIsCorrect()
            {
                result.AssertOk(42);
            }

            [Fact]
            public void ThenTheResultShouldHaveAValue()
            {
                result.HasValue.Should().BeTrue();
            }
        }

        public class WhenInvokingWhenOk
        {
            private readonly Result<int, string> result;

            public WhenInvokingWhenOk()
            {
                result = Result.Ok<int, string>(42);
            }

            [Fact]
            public void ThenTheActionShouldBeInvoked()
            {
                int invokedArg = 0;
                result.WhenOk(s => invokedArg = s);

                invokedArg.Should().Be(42);
            }

            [Fact]
            public void TheReturnedResultShouldBeTheSame()
            {
                result.WhenOk(_ => { }).Should().BeSameAs(result);
            }
        }

        public class WhenInvokingWhenError
        {
            private readonly Result<int, string> result;

            public WhenInvokingWhenError()
            {
                result = Result.Ok<int, string>(42);
            }

            [Fact]
            public void ThenTheActionShouldNotBeInvoked()
            {
                bool invoked = false;
                result.WhenError(_ => invoked = true);

                invoked.Should().BeFalse();
            }

            [Fact]
            public void TheReturnedResultShouldBeTheSame()
            {
                result.WhenError(_ => { }).Should().BeSameAs(result);
            }
        }

        public class WhenValueOrThrow
        {
            private readonly Result<int, Exception> result;

            public WhenValueOrThrow()
            {
                result = Result.Ok<int, Exception>(42);
            }

            [Fact]
            public void ThenAnExceptionIsNotThrown()
            {
                result.Invoking(r => r.ValueOrThrow()).ShouldNotThrow<Exception>();
            }

            [Fact]
            public void ThenTheResultIsNotChanged()
            {
                result.ValueOrThrow().Should().Be(42);
            }
        }

        public class WhenValueOr
        {
            private readonly int result;

            public WhenValueOr()
            {
                result = Result.Ok<int, string>(42).ValueOr(100);
            }

            [Fact]
            public void ThenTheOkValueIsReturned()
            {
                result.Should().Be(42);
            }
        }

        public class WhenValueOrCallback
        {
            private readonly int result;

            public WhenValueOrCallback()
            {
                result = Result.Ok<int, string>(42).ValueOr(() => 100);
            }

            [Fact]
            public void ThenTheOkValueIsReturned()
            {
                result.Should().Be(42);
            }
        }


        public class WhenValueOrErrorCallback
        {
            private readonly int result;

            public WhenValueOrErrorCallback()
            {
                result = Result.Ok<int, string>(42).ValueOr(e => 100);
            }

            [Fact]
            public void ThenTheOkValueIsReturned()
            {
                result.Should().Be(42);
            }
        }

        public class WhenUsedInALinqExpression
        {
            private readonly Result<int, string> result;

            public WhenUsedInALinqExpression()
            {
                result = Result.Ok<int, string>(42);
            }

            private static Result<int, string> ErrorsOnEvenValues(int x)
            {
                return x % 2 == 0 ? Result.Error<int, string>("Evens not allowed") : Result.Ok<int, string>(x);
            }

            [Fact]
            public void ThenSelectWorksAsExpected()
            {
                var r = from v in result
                        select v;

                r.AssertOk(42);

                r = from v in result
                    select v * 2;

                r.AssertOk(84);
            }

            [Fact]
            public void ThenSelectManyWorksAsExpected()
            {
                var r = from v1 in result
                        from v2 in ErrorsOnEvenValues(v1 + 1)
                        select v2;

                r.AssertOk(43);

                r = from v1 in result
                    from v2 in ErrorsOnEvenValues(v1)
                    select v2;

                r.HasValue.Should().BeFalse();
            }
        }
    }

    public class GivenAnError
    {
        public class WhenResultError
        {
            private readonly Result<int, string> result;

            public WhenResultError()
            {
                result = Result.Error<int, string>("Borked");
            }

            [Fact]
            public void ThenTheErrorIsCorrect()
            {
                result.AssertError("Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotHaveAValue()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenFlatMapping
        {
            private readonly Result<int, string> result;

            public WhenFlatMapping()
            {
                result = Result.Error<int, string>("Borked").FlatMap(r => Result.Ok<int, string>(r * 2));
            }

            [Fact]
            public void ThenTheFlatMapResultIsCorrect()
            {
                result.AssertError("Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotHaveAValue()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenFlatMappingToError
        {
            private readonly Result<int, string> result;

            public WhenFlatMappingToError()
            {
                result = Result.Error<int, string>("Borked").FlatMap(r => Result.Error<int, string>("More Borked"));
            }

            [Fact]
            public void ThenTheFlatMapResultIsCorrect()
            {
                result.AssertError("Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotHaveAValue()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenMapping
        {
            private readonly Result<int, string> result;

            public WhenMapping()
            {
                result = Result.Error<int, string>("Borked").Map(r => r * 2);
            }

            [Fact]
            public void ThenTheMapResultIsCorrect()
            {
                result.AssertError("Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotHaveAValue()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenMappingError
        {
            private readonly Result<int, string> result;

            public WhenMappingError()
            {
                result = Result.Error<int, string>("Borked").MapError(e => "More " + e);
            }

            [Fact]
            public void ThenTheMapErrorResultIsCorrect()
            {
                result.AssertError("More Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotHaveAValue()
            {
                result.HasValue.Should().BeFalse();
            }
        }

        public class WhenInvokingWhenOk
        {
            private readonly Result<int, string> result;

            public WhenInvokingWhenOk()
            {
                result = Result.Error<int, string>("Borked");
            }

            [Fact]
            public void ThenTheActionShouldNotBeInvoked()
            {
                bool invoked = false;
                result.WhenOk(_ => invoked = true);

                invoked.Should().BeFalse();
            }

            [Fact]
            public void TheReturnedResultShouldBeTheSame()
            {
                result.WhenOk(_ => { }).Should().BeSameAs(result);
            }
        }

        public class WhenInvokingWhenError
        {
            private readonly Result<int, string> result;

            public WhenInvokingWhenError()
            {
                result = Result.Error<int, string>("Borked");
            }

            [Fact]
            public void ThenTheActionShouldBeInvoked()
            {
                string invokedArg = "";
                result.WhenError(e => invokedArg = e);

                invokedArg.Should().Be("Borked");
            }

            [Fact]
            public void TheReturnedResultShouldBeTheSame()
            {
                result.WhenError(_ => { }).Should().BeSameAs(result);
            }
        }

        public class WhenValueOrThrow
        {
            private readonly Result<int, Exception> result;

            public WhenValueOrThrow()
            {
                result = Result.Error<int, Exception>(new Exception("Borked"));
            }

            [Fact]
            public void ThenAnExceptionIsThrown()
            {
                result.Invoking(r => r.ValueOrThrow()).ShouldThrow<Exception>().WithMessage("Borked");
            }
        }

        public class WhenValueOr
        {
            private readonly int result;

            public WhenValueOr()
            {
                result = Result.Error<int, string>("Borked").ValueOr(42);
            }

            [Fact]
            public void ThenTheDefaultValueIsReturned()
            {
                result.Should().Be(42);
            }
        }

        public class WhenValueOrCallback
        {
            private readonly int result;

            public WhenValueOrCallback()
            {
                result = Result.Error<int, string>("Borked").ValueOr(() => 42);
            }

            [Fact]
            public void ThenTheDefaultValueIsReturned()
            {
                result.Should().Be(42);
            }
        }


        public class WhenValueOrErrorCallback
        {
            private readonly int result;

            public WhenValueOrErrorCallback()
            {
                result = Result.Error<int, string>("42").ValueOr(e => int.Parse(e));
            }

            [Fact]
            public void ThenTheOkValueIsReturned()
            {
                result.Should().Be(42);
            }
        }

        public class WhenUsedInALinqExpression
        {
            private readonly Result<int, string> result;

            public WhenUsedInALinqExpression()
            {
                result = Result.Error<int, string>("Borked");
            }

            private static Result<int, string> ErrorsOnEvenValues(int x)
            {
                return x % 2 == 0 ? Result.Error<int, string>("Evens not allowed") : Result.Ok<int, string>(x);
            }

            [Fact]
            public void ThenSelectWorksAsExpected()
            {
                var r = from v in result
                        select v;

                r.AssertError("Borked");

                r = from v in result
                    select v * 2;

                r.AssertError("Borked");
            }

            [Fact]
            public void ThenSelectManyWorksAsExpected()
            {
                var r = from v1 in result
                        from v2 in ErrorsOnEvenValues(v1 + 1)
                        select v2;

                r.AssertError("Borked");

                r = from v1 in result
                    from v2 in ErrorsOnEvenValues(v1)
                    select v2;

                r.AssertError("Borked");
            }
        }
    }

    public class GivenANonThrowingMethod
    {
        public class WhenTrying
        {
            private readonly Result<int, Exception> result;

            public WhenTrying()
            {
                result = Result.Try(() => 42);
            }

            [Fact]
            public void ThenTheResultIsCorrect()
            {
                result.AssertOk(42);
            }

            [Fact]
            public void ThenTheResultShouldHaveAValue()
            {
                result.HasValue.Should().BeTrue();
            }
        }
    }

    public class GivenAThrowingMethod
    {
        public class WhenTrying
        {
            private readonly Result<int, Exception> result;

            public WhenTrying()
            {
                result = Result.Try<int, Exception>(() => throw new Exception("Borked"));
            }

            [Fact]
            public void ThenTheResultIsCorrect()
            {
                result.AssertError(e => e.Message == "Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotHaveAValue()
            {
                result.HasValue.Should().BeFalse();
            }
        }
    }
}
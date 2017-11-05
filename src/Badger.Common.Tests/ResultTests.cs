using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

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

        public class WhenApplyingSomeFunc
        {
            private readonly Result<int, string> _result;

            public WhenApplyingSomeFunc()
            {
                _result = Result.Ok<int, string>(42).Apply(Result.Ok<Func<int, int>, string>(f => f));
            }

            [Fact]
            public void ThenTheResultShouldBeCorrect()
            {
                _result.AssertOk(42);
            }
        }

        public class WhenApplyingErrorFunc
        {
            private readonly Result<int, string> _result;

            public WhenApplyingErrorFunc()
            {
                _result = Result.Ok<int, string>(42).Apply(Result.Error<Func<int, int>, string>("badger"));
            }

            [Fact]
            public void ThenTheResultShouldBeCorrect()
            {
                _result.AssertError("badger");
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

        public class WhenMatching
        {
            private readonly int _result;
            
            public WhenMatching()
            {
                _result = Result.Ok<int, string>(42)
                                .Match(ok: v => v, 
                                       error: e => 0);
            }

            [Fact]
            public void ThenTheOkMatchShouldBeCalled()
            {
                _result.Should().Be(42);
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

        public class WhenConvertingToAnEnumerable
        {
            private readonly IEnumerable<int> enumerable;

            public WhenConvertingToAnEnumerable()
            {
                enumerable = Result.Ok<int, string>(42).AsEnumerable();
            }

            [Fact]
            public void ThenTheEnumerableHasOneItem()
            {
                enumerable.Should().Equal(42);
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

                public class WhenApplyingSomeFunc
        {
            private readonly Result<int, string> _result;

            public WhenApplyingSomeFunc()
            {
                _result = Result.Error<int, string>("badger").Apply(Result.Ok<Func<int, int>, string>(f => f));
            }

            [Fact]
            public void ThenTheResultShouldBeCorrect()
            {
                _result.AssertError("badger");
            }
        }

        public class WhenApplyingErrorFunc
        {
            private readonly Result<int, string> _result;

            public WhenApplyingErrorFunc()
            {
                _result = Result.Error<int, string>("badger").Apply(Result.Error<Func<int, int>, string>("badger func"));
            }

            [Fact]
            public void ThenTheResultShouldBeCorrect()
            {
                _result.AssertError("badger func");
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

        public class WhenMatching
        {
            private readonly int _result;
            
            public WhenMatching()
            {
                _result = Result.Error<int, string>("badger")
                                .Match(ok: v => v, 
                                       error: e => 0);
            }

            [Fact]
            public void ThenTheErrorMatchShouldBeCalled()
            {
                _result.Should().Be(0);
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

        public class WhenConvertingToAnEnumerable
        {
            private readonly IEnumerable<int> enumerable;

            public WhenConvertingToAnEnumerable()
            {
                enumerable = Result.Error<int, string>("Borked").AsEnumerable();
            }

            [Fact]
            public void ThenTheEnumerableIsEmpty()
            {
                enumerable.Should().BeEmpty();
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

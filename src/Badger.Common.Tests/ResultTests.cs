using System;
using FluentAssertions;
using Xunit;

namespace Badger.Common.Tests
{
    public class GivenASuccess
    {
        public class WhenResultOk
        {
            private readonly Result<int, string> result;

            public WhenResultOk()
            {
                result = Result.Ok<int, string>(42);
            }

            [Fact]
            public void ThenTheSuccessIsCorrect()
            {
                result.Success.Should().Be(42);
            }

            [Fact]
            public void ThenTheErrorThrows()
            {
                var ex = Assert.Throws<InvalidOperationException>(() => result.Error);

                ex.Message.Should().Be("Error value not available on Success");
            }

            [Fact]
            public void ThenTheResultShouldBeSuccess()
            {
                result.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ThenTheResultShouldNotBeError()
            {
                result.IsError.Should().BeFalse();
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
                result.Success.Should().Be(84);
            }

            [Fact]
            public void ThenTheResultShouldBeSuccess()
            {
                result.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ThenTheResultShouldNotBeError()
            {
                result.IsError.Should().BeFalse();
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
                result.Error.Should().Be("Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotBeSuccess()
            {
                result.IsSuccess.Should().BeFalse();
            }

            [Fact]
            public void ThenTheResultShouldBeError()
            {
                result.IsError.Should().BeTrue();
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
                result.Success.Should().Be(84);
            }

            [Fact]
            public void ThenTheResultShouldBeSuccess()
            {
                result.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ThenTheResultShouldNotBeError()
            {
                result.IsError.Should().BeFalse();
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
                result.Success.Should().Be(42);
            }

            [Fact]
            public void ThenTheResultShouldBeSuccess()
            {
                result.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ThenTheResultShouldNotBeError()
            {
                result.IsError.Should().BeFalse();
            }
        }

        public class WhenSuccessOrThrow
        {
            private readonly Result<int, Exception> result;

            public WhenSuccessOrThrow()
            {
                result = Result.Ok<int, Exception>(42);
            }

            [Fact]
            public void ThenAnExceptionIsNotThrown()
            {
                result.Invoking(r => r.SuccessOrThrow()).ShouldNotThrow<Exception>();
            }

            [Fact]
            public void ThenTheResultIsNotChanged()
            {
                result.SuccessOrThrow().Should().Be(42);
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
                result.Error.Should().Be("Borked");
            }

            [Fact]
            public void ThenTheSuccessThrows()
            {
                var ex = Assert.Throws<InvalidOperationException>(() => result.Success);

                ex.Message.Should().Be("Success value not available on Erorr");
            }

            [Fact]
            public void ThenTheResultShouldNotBe()
            {
                result.IsSuccess.Should().BeFalse();
            }

            [Fact]
            public void ThenTheResultShouldBeError()
            {
                result.IsError.Should().BeTrue();
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
                result.Error.Should().Be("Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotBeBeSuccess()
            {
                result.IsSuccess.Should().BeFalse();
            }

            [Fact]
            public void ThenTheResultShouldBeError()
            {
                result.IsError.Should().BeTrue();
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
                result.Error.Should().Be("Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotBeSuccess()
            {
                result.IsSuccess.Should().BeFalse();
            }

            [Fact]
            public void ThenTheResultShouldBeError()
            {
                result.IsError.Should().BeTrue();
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
                result.Error.Should().Be("Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotBeSuccess()
            {
                result.IsSuccess.Should().BeFalse();
            }

            [Fact]
            public void ThenTheResultShouldBeError()
            {
                result.IsError.Should().BeTrue();
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
                result.Error.Should().Be("More Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotBeSuccess()
            {
                result.IsSuccess.Should().BeFalse();
            }

            [Fact]
            public void ThenTheResultShouldBeError()
            {
                result.IsError.Should().BeTrue();
            }
        }

        public class WhenSuccessOrThrow
        {
            private readonly Result<int, Exception> result;

            public WhenSuccessOrThrow()
            {
                result = Result.Error<int, Exception>(new Exception("Borked"));
            }

            [Fact]
            public void ThenAnExceptionIsThrown()
            {
                result.Invoking(r => r.SuccessOrThrow()).ShouldThrow<Exception>().WithMessage("Borked");
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
                result = Result.Try<int, Exception>(() => 42);
            }

            [Fact]
            public void ThenTheResultIsCorrect()
            {
                result.Success.Should().Be(42);
            }

            [Fact]
            public void ThenTheResultShouldBeSuccess()
            {
                result.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ThenTheResultShouldNotBeError()
            {
                result.IsError.Should().BeFalse();
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
                result.Error.Should().Match<Exception>(e => e.Message == "Borked");
            }

            [Fact]
            public void ThenTheResultShouldNotBeSuccess()
            {
                result.IsSuccess.Should().BeFalse();
            }

            [Fact]
            public void ThenTheResultShouldBeError()
            {
                result.IsError.Should().BeTrue();
            }
        }
    }
}
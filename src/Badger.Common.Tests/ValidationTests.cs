using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Badger.Common.Tests
{
    static class ValidationTestHelperExtensions
    {
        public static void AssertSuccess<T, TError>(this Validation<T, TError> validation, T success)
        {
            validation
                .WhenSuccess(s => s.Should().Be(success))
                .WhenError(e => Assert.True(false, $"expected Success: {success} but got errors: {e}"));
        }

        public static void AssertError<T, TError>(this Validation<T, TError> validation, params TError[] errors)
        {
            validation
                .WhenError(e => e.Should().BeEquivalentTo(errors))
                .WhenSuccess(s => Assert.True(false, $"expected error: {errors} but got OK: {s}"));
        }
    }

    public class ValidationTests
    {

        public class GivenASuccess
        {
            public class WhenSuccess
            {
                private readonly Validation<int, string> validation;

                public WhenSuccess()
                {
                    validation = Validation.Success<int, string>(42);
                }

                [Fact]
                public void ThenSuccessShouldBeTrue()
                {
                    validation.Success.Should().BeTrue();
                }
            }

            public class WhenFlatMapping
            {
                private readonly Validation<int, string> validation;

                public WhenFlatMapping()
                {
                    validation = Validation.Success<int, string>(42).FlatMap(r => Validation.Success<int, string>(r * 2));
                }

                [Fact]
                public void ThenTheFlatMapResultIsCorrect()
                {
                    validation.AssertSuccess(84);
                }

                [Fact]
                public void ThenSuccessShouldBeTrue()
                {
                    validation.Success.Should().BeTrue();
                }
            }

            public class WhenFlatMappingToError
            {
                private readonly Validation<int, string> validation;

                public WhenFlatMappingToError()
                {
                    validation = Validation.Success<int, string>(42).FlatMap(r => Validation.Error<int, string>("Borked"));
                }

                [Fact]
                public void ThenTheFlatMapResultIsCorrect()
                {
                    validation.AssertError("Borked");
                }

                [Fact]
                public void ThenSuccessShouldBeFalse()
                {
                    validation.Success.Should().BeFalse();
                }
            }

            public class WhenMapping
            {
                private readonly Validation<int, string> validation;

                public WhenMapping()
                {
                    validation = Validation.Success<int, string>(42).Map(r => r * 2);
                }

                [Fact]
                public void ThenTheMapResultIsCorrect()
                {
                    validation.AssertSuccess(84);
                }

                [Fact]
                public void ThenSuccessShouldBeTrue()
                {
                    validation.Success.Should().BeTrue();
                }
            }

            public class WhenApplyingSuccessFunc
            {
                private readonly Validation<int, string> validation;

                public WhenApplyingSuccessFunc()
                {
                    validation = Validation.Success<int, string>(42).Apply(Validation.Success<Func<int, int>, string>(f => f));
                }

                [Fact]
                public void ThenTheValidationShouldBeCorrect()
                {
                    validation.AssertSuccess(42);
                }
            }

            public class WhenApplyingErrorFunc
            {
                private readonly Validation<int, string> validation;

                public WhenApplyingErrorFunc()
                {
                    validation = Validation.Success<int, string>(42).Apply(Validation.Error<Func<int, int>, string>("borked"));
                }

                [Fact]
                public void ThenTheValidationShouldBeCorrect()
                {
                    validation.AssertError("borked");
                }
            }

            public class WhenMappingError
            {
                private readonly Validation<int, string> validation;

                public WhenMappingError()
                {
                    validation = Validation.Success<int, string>(42).MapError(e => "More " + e);
                }

                [Fact]
                public void ThenTheErrorMapValidationIsCorrect()
                {
                    validation.AssertSuccess(42);
                }

                [Fact]
                public void ThenSuccessShouldBeTrue()
                {
                    validation.Success.Should().BeTrue();
                }
            }

            public class WhenInvokingWhenSuccess
            {
                private readonly Validation<int, string> validation;

                public WhenInvokingWhenSuccess()
                {
                    validation = Validation.Success<int, string>(42);
                }

                [Fact]
                public void ThenTheActionShouldBeInvoked()
                {
                    int invokedArg = 0;
                    validation.WhenSuccess(s => invokedArg = s);

                    invokedArg.Should().Be(42);
                }

                [Fact]
                public void ThenTheReturnedValidationShouldBeTheSame()
                {
                    validation.WhenSuccess(_ => { }).Should().BeSameAs(validation);
                }
            }

            public class WhenInvokingWhenError
            {
                private readonly Validation<int, string> validation;

                public WhenInvokingWhenError()
                {
                    validation = Validation.Success<int, string>(42);
                }

                [Fact]
                public void ThenTheActionShouldNotBeInvoked()
                {
                    bool invoked = false;
                    validation.WhenError(_ => invoked = true);

                    invoked.Should().BeFalse();
                }

                [Fact]
                public void TheReturnedValidationShouldBeTheSame()
                {
                    validation.WhenError(_ => { }).Should().BeSameAs(validation);
                }
            }

            public class WhenMatching
            {
                private readonly int result;

                public WhenMatching()
                {
                    result = Validation.Success<int, string>(42)
                                       .Match(success: v => v,
                                              error: e => 0);
                }

                [Fact]
                public void ThenTheSuccessMatchShouldBeCalled()
                {
                    result.Should().Be(42);
                }
            }
        }

        public class GivenAnError
        {
            public class WhenSuccess
            {
                private readonly Validation<int, string> validation;

                public WhenSuccess()
                {
                    validation = Validation.Error<int, string>("Borked");
                }

                [Fact]
                public void ThenSuccessShouldBeFalse()
                {
                    validation.Success.Should().BeFalse();
                }
            }

            public class WhenFlatMapping
            {
                private readonly Validation<int, string> validation;

                public WhenFlatMapping()
                {
                    validation = Validation.Error<int, string>("Borked").FlatMap(r => Validation.Success<int, string>(r * 2));
                }

                [Fact]
                public void ThenTheFlatMapResultIsCorrect()
                {
                    validation.AssertError("Borked");
                }

                [Fact]
                public void ThenSuccessShouldBeFalse()
                {
                    validation.Success.Should().BeFalse();
                }
            }

            public class WhenFlatMappingToError
            {
                private readonly Validation<int, string> validation;

                public WhenFlatMappingToError()
                {
                    validation = Validation.Error<int, string>("Borked").FlatMap(r => Validation.Error<int, string>("More Borked"));
                }

                [Fact]
                public void ThenTheFlatMapResultIsCorrect()
                {
                    validation.AssertError("Borked");
                }

                [Fact]
                public void ThenSuccessShouldBeFalse()
                {
                    validation.Success.Should().BeFalse();
                }
            }

            public class WhenMapping
            {
                private readonly Validation<int, string> validation;

                public WhenMapping()
                {
                    validation = Validation.Error<int, string>("Borked").Map(r => r * 2);
                }

                [Fact]
                public void ThenTheMapResultIsCorrect()
                {
                    validation.AssertError("Borked");
                }

                [Fact]
                public void ThenSuccessShouldBeFalse()
                {
                    validation.Success.Should().BeFalse();
                }
            }

            public class WhenApplyingSuccessFunc
            {
                private readonly Validation<int, string> validation;

                public WhenApplyingSuccessFunc()
                {
                    validation = Validation.Error<int, string>("Borked").Apply(Validation.Success<Func<int, int>, string>(f => f));
                }

                [Fact]
                public void ThenTheValidationShouldBeCorrect()
                {
                    validation.AssertError("Borked");
                }
            }

            public class WhenApplyingErrorFunc
            {
                private readonly Validation<int, string> validation;

                public WhenApplyingErrorFunc()
                {
                    validation = Validation.Error<int, string>("Borked").Apply(Validation.Error<Func<int, int>, string>("More Borked"));
                }

                [Fact]
                public void ThenTheValidationShouldBeCorrect()
                {
                    validation.AssertError("Borked", "More Borked");
                }
            }

            public class WhenMappingError
            {
                private readonly Validation<int, string> validation;

                public WhenMappingError()
                {
                    validation = Validation.Error<int, string>("Borked").MapError(e => "More " + e);
                }

                [Fact]
                public void ThenTheErrorMapValidationIsCorrect()
                {
                    validation.AssertError("More Borked");
                }

                [Fact]
                public void ThenSuccessShouldBeFalse()
                {
                    validation.Success.Should().BeFalse();
                }
            }

            public class WhenInvokingWhenSuccess
            {
                private readonly Validation<int, string> validation;

                public WhenInvokingWhenSuccess()
                {
                    validation = Validation.Error<int, string>("Borked");
                }

                [Fact]
                public void ThenTheActionShouldNotBeInvoked()
                {
                    bool invoked = false;
                    validation.WhenSuccess(_ => invoked = true);

                    invoked.Should().BeFalse();
                }

                [Fact]
                public void ThenTheReturnedValidationShouldBeTheSame()
                {
                    validation.WhenSuccess(_ => { }).Should().BeSameAs(validation);
                }
            }

            public class WhenInvokingWhenError
            {
                private readonly Validation<int, string> validation;

                public WhenInvokingWhenError()
                {
                    validation = Validation.Error<int, string>("Borked");
                }

                [Fact]
                public void ThenTheActionShouldBeInvoked()
                {
                    string[] invokedArgs = null;
                    validation.WhenError(e => invokedArgs = e.ToArray());

                    invokedArgs.Should().BeEquivalentTo("Borked");
                }

                [Fact]
                public void TheReturnedValidationShouldBeTheSame()
                {
                    validation.WhenError(_ => { }).Should().BeSameAs(validation);
                }
            }

            public class WhenMatching
            {
                private readonly int result;

                public WhenMatching()
                {
                    result = Validation.Error<int, string>("Borked")
                                       .Match(success: v => 0,
                                              error: e => 42);
                }

                [Fact]
                public void ThenTheErrorMatchShouldBeCalled()
                {
                    result.Should().Be(42);
                }
            }
        }
    }
}
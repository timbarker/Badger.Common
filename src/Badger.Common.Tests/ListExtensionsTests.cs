using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Badger.Common.Tests
{
    public class GivenAnEmptyList
    {
        public class WhenFindingAValue
        {
            private readonly Optional<int> _result;

            public WhenFindingAValue()
            {
                var list = new List<int>();

                _result = list.FindValue(i => true);
            }

            [Fact]
            public void ThenNoneIsReturned()
            {
                _result.HasValue.Should().BeFalse();
            }
        }

        public class WhenPicking
        {
            private readonly Optional<int> _result;

            public WhenPicking()
            {
                var list = new List<int>();

                _result = list.Pick(i => Optional.None<int>());
            }

            [Fact]
            public void ThenNoneIsReturned()
            {
                _result.HasValue.Should().BeFalse();
            }
        }
    }

    public class GivenANonEmptyList
    {
        public class WhenFindValuePredicateReturnsTrue
        {
            private readonly Optional<int> _result;

            public WhenFindValuePredicateReturnsTrue()
            {
                var list = new List<int> { 42 };

                _result = list.FindValue(v => true);
            }

            [Fact]
            public void ThenSomeIsReturned()
            {
                _result.HasValue.Should().BeTrue();
                _result.WhenSome(v => v.Should().Be(42));
            }
        }

        public class WhenFindValuePredicateReturnsFalse
        {
            private readonly Optional<int> _result;

            public WhenFindValuePredicateReturnsFalse()
            {
                var list = new List<int> { 42 };

                _result = list.FindValue(i => false);
            }

            [Fact]
            public void ThenNoneIsReturned()
            {
                _result.HasValue.Should().BeFalse();
            }
        }

        public class WhenPickingReturnsNone
        {
            private readonly Optional<int> _result;

            public WhenPickingReturnsNone()
            {
                var list = new List<int> { 42 };

                _result = list.Pick(i => Optional.None<int>()); 
            }

            [Fact]
            public void ThenNoneIsReturned()
            {
                _result.HasValue.Should().BeFalse();
            }
        }

        public class WhenPickingReturnsSome
        {
            private readonly Optional<int> _result;

            public WhenPickingReturnsSome()
            {
                var list = new List<int> { 42 };

                _result = list.Pick(i => Optional.Some(i));
            }

            [Fact]
            public void ThenSomeIsReturned()
            {
                _result.HasValue.Should().BeTrue();
                _result.WhenSome(v => v.Should().Be(42));
            }
        }
    }
}
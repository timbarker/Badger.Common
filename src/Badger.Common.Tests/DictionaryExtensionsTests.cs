using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Badger.Common.Tests
{
    public class GivenAnEmptyDictionary
    {
        public class WhenFinding
        {
            private readonly Optional<int> _result;

            public WhenFinding()
            {
                var dictionary = new Dictionary<string, int>();
                _result = dictionary.Find("Badger");    
            }

            [Fact]
            public void ThenNoneIsReturned()
            {
                _result.HasValue.Should().BeFalse();
            }
        }

        public class WhenFindingAKey
        {
            private readonly Optional<string> _result;

            public WhenFindingAKey()
            {
                var dictionary = new Dictionary<string, int>();
                _result = dictionary.FindKey((k, v) => true);
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
                var dictionary = new Dictionary<string, int>();
                _result = dictionary.Pick((k, v) => Optional.Some(42));
            }

            [Fact]
            public void ThenNoneIsReturned()
            {
                _result.HasValue.Should().BeFalse();
            }
        }
    }

    public class GivenANonEmptyDictionary
    {
        public class WhenFindingAValidKey
        {
            private readonly Optional<int> _result;

            public WhenFindingAValidKey()
            {
                var dictionary = new Dictionary<string, int> { ["Badger"] = 42 };

                _result = dictionary.Find("Badger");
            }

            [Fact]
            public void ThenSomeIsReturned()
            {
                _result.HasValue.Should().BeTrue();
                _result.WhenSome(v => v.Should().Be(42));
            }
        }

        public class WhenFindingAnInvalidKey
        {
            private readonly Optional<int> _result;

            public WhenFindingAnInvalidKey()
            {
                var dictionary = new Dictionary<string, int> { ["Badger"] = 42 };

                _result = dictionary.Find("Not A Badger");
            }

            [Fact]
            public void ThenNoneIsReturned()
            {
                _result.HasValue.Should().BeFalse();
            }
        }

        public class WhenFindKeyPredicateReturnsFalse
        {
            private readonly Optional<string> _result;

            public WhenFindKeyPredicateReturnsFalse()
            {
                var dictionary = new Dictionary<string, int> { ["Badger"] = 42 };

                _result = dictionary.FindKey((k, v) => false);
            }

            [Fact]
            public void ThenNoneIsReturned()
            {
                _result.HasValue.Should().BeFalse();
            }
        }

        public class WhenFindKeyPredicateReturnsTrue
        {
            private readonly Optional<string> _result;

            public WhenFindKeyPredicateReturnsTrue()
            {
                var dictionary = new Dictionary<string, int> { ["Badger"] = 42 };

                _result = dictionary.FindKey((k, v) => true);
            }

            [Fact]
            public void ThenSomeIsReturned()
            {
                _result.HasValue.Should().BeTrue();
                _result.WhenSome(v => v.Should().Be("Badger"));
            }
        }

        public class WhenPickingRetunsNone
        {
            private readonly Optional<int> _result;

            public WhenPickingRetunsNone()
            {
                var dictionary = new Dictionary<string, int> { ["Badger"] = 42 };

                _result = dictionary.Pick((k, v) => Optional.None<int>());
            }

            [Fact]
            public void ThenNoneIsReturned()
            {
                _result.HasValue.Should().BeFalse();
            }
        }

        public class WhenPickingRetunsSome
        {
            private readonly Optional<string> _result;

            public WhenPickingRetunsSome()
            {
                var dictionary = new Dictionary<string, int> { ["Badger"] = 42 };

                _result = dictionary.Pick((k, v) => Optional.Some(k + v));
            }

            [Fact]
            public void ThenSomeIsReturned()
            {
                _result.HasValue.Should().BeTrue();
                _result.WhenSome(v => v.Should().Be("Badger42"));
            }
        }
    }
}
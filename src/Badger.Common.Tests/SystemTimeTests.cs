using System;
using Xunit;
using FluentAssertions;

namespace Badger.Common.Tests
{
    public class GivenAnUnfrozenSystemTime
    {
        public class WhenFreezingWithAnExplcitTime : IDisposable
        {
            private readonly DateTime frozenTime = new DateTime(2017, 10, 8, 12, 22, 33);
            private readonly IDisposable reset;
            public WhenFreezingWithAnExplcitTime()
            {
                reset = SystemTime.Freeze(frozenTime);
            }

            public void Dispose()
            {
                reset.Dispose();
            }

            [Fact]
            public void ThenTheTimeIsFrozen()
            {
                SystemTime.UtcNow.Should().Equals(frozenTime);
            }
        }

        public class WhenFreezingWitWithoutAnExplicitTime : IDisposable
        {
            private readonly DateTime frozenTime;
            private readonly IDisposable reset;
            public WhenFreezingWitWithoutAnExplicitTime()
            {
                frozenTime = DateTime.UtcNow;
                reset = SystemTime.Freeze();
            }

            public void Dispose()
            {
                reset.Dispose();
            }

            [Fact]
            public void ThenTheTimeIsFrozen()
            {
                SystemTime.UtcNow.Should().BeCloseTo(frozenTime);
            }
        }
    }

    public class GivenAnFrozenSystemTime
    {
        public class WhenUnfreezing
        {        
            private readonly DateTime frozenTime = new DateTime(2017, 10, 8, 12, 22, 33);

            public WhenUnfreezing()
            {
                var reset = SystemTime.Freeze(frozenTime);

                reset.Dispose();
            }

            [Fact]
            public void ThenTheTimeIsUnfrozen()
            {
                SystemTime.UtcNow.Should().BeCloseTo(DateTime.UtcNow);
            }
        }

        public class WhenFreezing : IDisposable
        {
            private readonly DateTime firstFrozenTime = new DateTime(2017, 10, 8, 12, 22, 33);
            private readonly DateTime secondFrozenTime = new DateTime(2017, 10, 9, 12, 22, 33);

            private readonly IDisposable reset;
            public WhenFreezing()
            {
                SystemTime.Freeze(firstFrozenTime);
                reset = SystemTime.Freeze(secondFrozenTime);
            }

            public void Dispose()
            {
                reset.Dispose();
            }

            [Fact]
            public void ThenTheTimeIsFrozen()
            {
                SystemTime.UtcNow.Should().Equals(secondFrozenTime);
            }
        }

        public class WhenUnfreezingMultipleTimes
        {        
            private readonly DateTime frozenTime = new DateTime(2017, 10, 8, 12, 22, 33);
            private readonly IDisposable reset;

            public WhenUnfreezingMultipleTimes()
            {
                reset = SystemTime.Freeze(frozenTime);

                reset.Dispose();
            }

            [Fact]
            public void DoesNotThrow()
            {
                reset.Invoking(r => r.Dispose()).ShouldNotThrow<Exception>();
            }
        }
    }
}

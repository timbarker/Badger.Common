using FluentAssertions;
using Xunit;

namespace Badger.Common.Tests
{
    public class GivenAnActionWrappedInADisposable
    {
        public class WhenDisposing
        {
            [Fact]
            public void ThenTheActionIsInvoked()
            {
                bool invoked = false;
                var disposable = Disposable.From(() => invoked = true);

                disposable.Dispose();

                invoked.Should().BeTrue();
            }
        }

        public class WhenDisposingMultipleTimes
        {
            [Fact]
            public void ThenTheActionIsOnlyInvokedOnce()
            {
                int invokedCount = 0;
                var disposable = Disposable.From(() => invokedCount++);

                disposable.Dispose();
                disposable.Dispose();

                invokedCount.Should().Be(1);
            }
        }

        public class WhenAggregatingDisposables
        {
            [Fact]
            public void ThenTheDisposblesAreAllCalled()
            {
                int invokedCount = 0;
                var disposbale = Disposable.From(Disposable.From(() => invokedCount++), Disposable.From(() => invokedCount++));

                disposbale.Dispose();

                invokedCount.Should().Be(2);
            }
        }
    }
}

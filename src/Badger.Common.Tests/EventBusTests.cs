using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Badger.Common.Tests
{
    public class GivenAnEventBus
    {
        public class WhenSubscribing
        {
            private List<string> _raisedEvents = new List<string>();
            public WhenSubscribing()
            {
                var bus = new EventBus();

                bus.Subscribe<string>(s => _raisedEvents.Add(s));

                bus.Publish("Badger");
            }

            [Fact]
            public void ThenTheSubscriptionIsCalled()
            {
                _raisedEvents.Should().Equal("Badger");
            }
        }

        public class WhenMultipleSubscribers
        {
            private List<string> _raisedEvents = new List<string>();
            public WhenMultipleSubscribers()
            {
                var bus = new EventBus();

                bus.Subscribe<string>(s => _raisedEvents.Add("Sub 1: " + s));
                bus.Subscribe<string>(s => _raisedEvents.Add("Sub 2: " + s));


                bus.Publish("Badger");
            }

            [Fact]
            public void ThenTheSubscriptionsAreAllCalled()
            {
                _raisedEvents.Should().Equal("Sub 1: Badger", "Sub 2: Badger");
            }
        }

        public class WhenMultipleDifferentSubscribers
        {
            private List<string> _raisedStringEvents = new List<string>();
            private List<int> _raisedIntEvents = new List<int>();

            public WhenMultipleDifferentSubscribers()
            {
                var bus = new EventBus();

                bus.Subscribe<string>(s => _raisedStringEvents.Add(s));
                bus.Subscribe<int>(s => _raisedIntEvents.Add(s));


                bus.Publish("Badger");
                bus.Publish(42);
            }

            [Fact]
            public void ThenTheSubscriptionsAreAllCalled()
            {
                _raisedStringEvents.Should().Equal("Badger");
                _raisedIntEvents.Should().Equal(42);
            }
        }

        public class WhenUbsubsribing
        {
            private List<string> _raisedEvents = new List<string>();

            public WhenUbsubsribing()
            {
                var bus = new EventBus();

                var subscription = bus.Subscribe<string>(s => _raisedEvents.Add(s));
                
                subscription.Dispose();

                bus.Publish("Badger");
            }

            [Fact]
            public void ThenTheSubscriptionIsNotCalled()
            {
                _raisedEvents.Should().BeEmpty();
            }
        }

        public class WhenUbsubsribingWithMultipleSubscribers
        {
            private List<string> _raisedEvents = new List<string>();

            public WhenUbsubsribingWithMultipleSubscribers()
            {
                var bus = new EventBus();

                var subscription = bus.Subscribe<string>(s => _raisedEvents.Add(s));
                bus.Subscribe<string>(s => _raisedEvents.Add("Sub 2: " + s));
                
                subscription.Dispose();

                bus.Publish("Badger");
            }

            [Fact]
            public void ThenTheRemainingSubscriptionCalled()
            {
                _raisedEvents.Should().Equal("Sub 2: Badger");
            }
        }

        public class WhenUnsubscribingInHandler
        {
            private List<string> _raisedEvents = new List<string>();

            public WhenUnsubscribingInHandler()
            {
                var bus = new EventBus();
                IDisposable subscription = null;
                subscription =  bus.Subscribe<string>(s => {
                    _raisedEvents.Add(s);
                    subscription.Dispose();
                });
            
                bus.Publish("Badger");
                bus.Publish("Badger");
            }

            [Fact]
            public void ThenTheSubscriptionIsCalledOnce()
            {
                _raisedEvents.Should().Equal("Badger");
            }
        }

        public class WhenSubscribingAnObject
        {
            private readonly MyObject _object;

            class MyObject
            {
                public List<string> RaisedStringEvents = new List<string>();
                public List<int> RaisedIntEvents = new List<int>();

                [Subscribe]
                public void Handles(string s)
                {
                    RaisedStringEvents.Add(s);
                }

                [Subscribe]
                private void Handles(int i)
                {
                    RaisedIntEvents.Add(i);
                }
            }

            public WhenSubscribingAnObject()
            {
                _object = new MyObject();

                var bus = new EventBus();
                bus.Subscribe(_object);

                bus.Publish("Badger");
                bus.Publish(42);
            }

            [Fact]
            public void ThenTheSubscriptionIsCalled()
            {
                _object.RaisedStringEvents.Should().Equal("Badger");
                _object.RaisedIntEvents.Should().Equal(42);
            }
        }

        public class WhenUnsubscribingAnObject
        {
            private readonly MyObject _object;

            class MyObject
            {
                public List<string> RaisedStringEvents = new List<string>();
                public List<int> RaisedIntEvents = new List<int>();

                [Subscribe]
                public void Handles(string s)
                {
                    RaisedStringEvents.Add(s);
                }

                [Subscribe]
                public void Handles(int i)
                {
                    RaisedIntEvents.Add(i);
                }
            }

            public WhenUnsubscribingAnObject()
            {
                _object = new MyObject();

                var bus = new EventBus();
                var subscription = bus.Subscribe(_object);
                subscription.Dispose();

                bus.Publish("Badger");
                bus.Publish(42);
            }

            [Fact]
            public void ThenTheSubscriptionIsNotCalled()
            {
                _object.RaisedStringEvents.Should().BeEmpty();
                _object.RaisedIntEvents.Should().BeEmpty();
            }
        }

        public class WhenSubscribingAnObjectWithDuplicateSubscriptions
        {
            private readonly MyObject _object;

            class MyObject
            {
                public List<string> RaisedStringEvents = new List<string>();

                [Subscribe]
                public void Handles(string s)
                {
                    RaisedStringEvents.Add("Handles " + s);
                }

                [Subscribe]
                public void HandlesAgain(string s)
                {
                    RaisedStringEvents.Add("HandlesAgain " + s);

                }
            }

            public WhenSubscribingAnObjectWithDuplicateSubscriptions()
            {
                _object = new MyObject();

                var bus = new EventBus();
                bus.Subscribe(_object);

                bus.Publish("Badger");
            }

            [Fact]
            public void ThenTheSubscriptionsAreCalled()
            {
                _object.RaisedStringEvents.Should().Equal("Handles Badger", "HandlesAgain Badger");
            }
        }

        public class WhenSubscribingAnObjectWithInvalidSubscriptions
        {
            private readonly MyObject _object;

            class MyObject
            {
                public List<string> RaisedStringEvents = new List<string>();

                [Subscribe]
                public void Handles(string s, int other)
                {
                    RaisedStringEvents.Add(s);
                }

                [Subscribe]
                public int Handles(string s)
                {
                    RaisedStringEvents.Add(s);
                    return 42;
                }
            }

            public WhenSubscribingAnObjectWithInvalidSubscriptions()
            {
                _object = new MyObject();

                var bus = new EventBus();
                bus.Subscribe(_object);

                bus.Publish("Badger");
            }

            [Fact]
            public void ThenTheSubscriptionsAreNotCalled()
            {
                _object.RaisedStringEvents.Should().BeEmpty();
            }
        }
    }
}

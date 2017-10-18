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

        public class WhenEventIsNotHandled
        {
            private DeadEvent _deadEvent;
            public WhenEventIsNotHandled()
            {
                var bus = new EventBus();

                bus.Subscribe<DeadEvent>(e => _deadEvent = e);

                bus.Publish("badger");
            }

            [Fact]
            public void ThenADeadEventIsPublished()
            {
                _deadEvent.Should().NotBeNull();
                _deadEvent.Event.Should().Equals("badger");
            }
        }

        public class WhenAPreviouslySubscribedEventIsNotHandled
        {
            private DeadEvent _deadEvent;
            public WhenAPreviouslySubscribedEventIsNotHandled()
            {
                var bus = new EventBus();

                bus.Subscribe<DeadEvent>(e => _deadEvent = e);
                bus.Subscribe<string>(_ => {}).Dispose();

                bus.Publish("badger");
            }

            [Fact]
            public void ThenADeadEventIsPublished()
            {
                _deadEvent.Should().NotBeNull();
                _deadEvent.Event.Should().Be("badger");
            }
        }

        public class WhenAnExceptionIsThrownFromASubscription
        {
            private List<string> raisedEvents = new List<string>();
            private EventBus.ErrorArgs errorEvent; 
            
            public WhenAnExceptionIsThrownFromASubscription()
            {
                var bus = new EventBus();

                bus.Error += (s, e) => errorEvent = e;

                bus.Subscribe<string>(s => raisedEvents.Add("Before " + s));  
                bus.Subscribe<string>(s => throw new Exception("Error: " + s));
                bus.Subscribe<string>(s => raisedEvents.Add("After " + s));  
        
                bus.Publish("Badger");
            }

            [Fact]
            public void ThenTheErrorEventIsRaised()
            {
                errorEvent.Should().NotBeNull();
                errorEvent.Event.Should().Be("Badger");
                errorEvent.Exception.Message.Should().Be("Error: Badger");
            }

            [Fact]
            public void TheOtherSubscribersAreStillInvoked()
            {
                raisedEvents.Should().Equal("Before Badger", "After Badger");
            }
        }

        public class WhenAnExceptionIsThrownFromDeadEventSubscirption
        {
            private EventBus.ErrorArgs errorEvent; 
            public WhenAnExceptionIsThrownFromDeadEventSubscirption()
            {
                var bus = new EventBus();
                bus.Error += (s, e) => errorEvent = e;

                bus.Subscribe<DeadEvent>(e => throw new Exception("Error: " + e.Event));
                
                bus.Publish("Badger");
            }

            [Fact]
            public void ThenTheErrorEventIsRaised()
            {
                errorEvent.Should().NotBeNull();
                errorEvent.Event.As<DeadEvent>().Event.Should().Be("Badger");
                errorEvent.Exception.Message.Should().Be("Error: Badger");
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

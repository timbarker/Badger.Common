using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Badger.Common
{

    public class EventBus : IEventBus
    {
        interface IHandler 
        {
            void Call(object @event);

            Type Type { get;}
        }

        class ActionHandler<T> : IHandler
        {
            private Action<T> _handler;

            public Type Type => typeof(T);

            public ActionHandler(Action<T> handler)
            {
                _handler = handler;
            }
            public void Call(object @event)
            {
                _handler.Invoke((T)@event);
            }
        }

        class MethodHandler : IHandler
        {
            private readonly object _instance;
            private readonly MethodInfo _method;

            public MethodHandler(object instance, MethodInfo method)
            {
                _instance = instance;
                _method = method;
            }
            public Type Type => _method.GetParameters()[0].ParameterType;

            public void Call(object @event)
            {
                _method.Invoke(_instance, new[] { @event });
            }
        }

        private ConcurrentDictionary<Type, ImmutableList<IHandler>> _eventHandlers;
        
        public EventBus()
        {
            _eventHandlers = new ConcurrentDictionary<Type, ImmutableList<IHandler>>();
        }

        public IDisposable Subscribe(object o)
        {
            return Disposable.From(o.GetType()
                                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                    .Where(m => m.GetCustomAttribute<SubscribeAttribute>() != null && 
                                                m.GetParameters().Length == 1 && 
                                                m.ReturnType == typeof(void))
                                    .Select(m => Subscribe(new MethodHandler(o, m)))
                                    .ToArray());         
        }

        public IDisposable Subscribe<T>(Action<T> subscription)
        {
            var handler = new ActionHandler<T>(subscription);

            return Subscribe( handler);
        }

        private IDisposable Subscribe(IHandler handler)
        {
            _eventHandlers.AddOrUpdate(
                handler.Type,
                ImmutableList.Create(handler as IHandler),
                (_, handlers) => handlers.Add(handler)
            );

            return Disposable.From(() =>
            {
                while (true)
                {
                    if (!_eventHandlers.TryGetValue(handler.Type, out var handlers)) break;

                    var newHandlers = handlers.Remove(handler);

                    if (_eventHandlers.TryUpdate(handler.Type, newHandlers, handlers)) break;
                }
            });
        }

        public void Publish<T>(T @event)
        {
            if (!_eventHandlers.TryGetValue(typeof(T), out var handlers)) return;

            handlers.ForEach(h => h.Call(@event));
        }
    }
}

using System;

namespace Badger.Common
{
    interface IEventBus
    {
        IDisposable Subscribe<T>(Action<T> subscription);
        IDisposable Subscribe(object o);

        void Publish<T>(T @event);
    }
}

namespace Badger.Common
{
    public class DeadEvent
    {
        public DeadEvent(object @event)
        {
            Event = @event;
        }

        public object Event { get; }
    }
}

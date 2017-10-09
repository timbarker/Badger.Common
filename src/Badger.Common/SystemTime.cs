using System;

namespace Badger.Common
{
    public static class SystemTime
    {
        private static readonly Func<DateTime> _defaultTimeSource = () => DateTime.UtcNow;
        private static Func<DateTime> _timeSource = _defaultTimeSource;
        public static DateTime UtcNow => _timeSource();

        public static IDisposable Freeze(DateTime? time = null)
        {
            var frozen = time ?? DateTime.UtcNow;
            _timeSource = () => frozen;
            return Resetter.Instance;
        }

        private static void Reset() => _timeSource = _defaultTimeSource;

        private class Resetter : IDisposable
        {
            public static readonly Resetter Instance = new Resetter();
            
            public void Dispose()
            {
                SystemTime.Reset();
            }
        }
    }
}

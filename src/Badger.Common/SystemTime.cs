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
            return Disposable.From(() => _timeSource = _defaultTimeSource);
        }
    }
}

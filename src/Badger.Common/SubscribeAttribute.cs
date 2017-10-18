using System;

namespace Badger.Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SubscribeAttribute : Attribute
    {
    }
}

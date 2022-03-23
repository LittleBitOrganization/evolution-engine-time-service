using System;
using System.Collections;

namespace LittleBit.Modules.TimeServiceModule
{
    public class DeviceTimeGetter : ITimeGetter
    {
        public IEnumerator GetCurrentTime(Action<DateTime> callback, Action error)
        {
            callback?.Invoke(DateTime.UtcNow);

            yield return null;
        }
    }
}
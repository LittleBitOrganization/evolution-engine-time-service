using System;
using System.Collections;

namespace LittleBit.Modules.TimeServiceModule
{
    public interface ITimeGetter
    {
        public IEnumerator GetCurrentTime(Action<DateTime> callback, Action error);
    }
}
using System;

namespace LittleBit.Modules.TimeServiceModule
{
    public interface ITimeService
    {
        TimeData TimeStorageData { get; }
        
        event Action<long> OnTimeInitialized;
    }
}
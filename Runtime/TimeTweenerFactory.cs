using System;

namespace LittleBit.Modules.TimeServiceModule
{
    public class TimeTweenerFactory
    {
        private readonly ITimeService _timeService;

        public TimeTweenerFactory(ITimeService timeService) => 
            _timeService = timeService;

        public TimeTweener Create(Time startTime, float duration, bool ignoreTimeScale = false) => 
            new TimeTweener(_timeService, startTime, duration, ignoreTimeScale);

        public TimeTweener CreateFromNow(float duration, bool ignoreTimeScale = false) => 
            new TimeTweener(_timeService, new Time(GetCurrentTime()), duration, ignoreTimeScale);

        private DateTime GetCurrentTime() => 
            _timeService.TimeStorageData.ReadDateTime(TimeData.CurrentTimeName);
    }
}

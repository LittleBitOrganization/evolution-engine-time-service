using System;

namespace LittleBit.Modules.TimeServiceModule
{
    public class TimeTweenerFactory
    {
        private readonly ITimeService _timeService;

        public TimeTweenerFactory(ITimeService timeService) => 
            _timeService = timeService;

        public TimeTweener Create(Time startTime, float duration) => 
            new TimeTweener(_timeService, startTime, duration);

        public TimeTweener CreateFromNow(float duration) => 
            new TimeTweener(_timeService, new Time(GetCurrentTime()), duration);

        private DateTime GetCurrentTime() => 
            _timeService.TimeStorageData.ReadDateTime(TimeData.CurrentTimeName);
    }
}
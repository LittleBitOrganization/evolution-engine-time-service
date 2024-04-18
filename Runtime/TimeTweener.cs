using System;
using DG.Tweening;
using LittleBit.Modules.SequenceLogicModule;

namespace LittleBit.Modules.TimeServiceModule
{
    public class TimeTweener
    {
        private readonly ITimeService _timeService;
       
        private readonly double _duration;
        private readonly bool _ignoreTimeScale;

        public Time StartTime { get; private set; }
        
        public event Action OnComplete;
        public event Action<TimeTweenerData> OnUpdate;
        public TimeTweenerData TweenerData { get; private set; }
        
        private DateTime _startTime;
        private DateTime _endTime;


        public TimeTweener(ITimeService timeService, Time startTime, double duration, bool ignoreTimeScale = false)
        {
            StartTime = startTime;
            
            _timeService = timeService;
            _duration = duration;
            _ignoreTimeScale = ignoreTimeScale;
            Init();
            TweenerData = new TimeTweenerData(duration, 0);
        }


        private void Init()
        {
            _startTime = TimeData.ConvertStringToDateTime(StartTime.Value);
            _endTime = _startTime + TimeSpan.FromMilliseconds(_duration * 1000);
        }
        
        public void SkipTime(double seconds, Action onChangeStartTime = null)
        {
            _startTime -= TimeSpan.FromMilliseconds(seconds * 1000);
            _endTime = _startTime + TimeSpan.FromMilliseconds(_duration * 1000);
            StartTime.SetNewDataTime(_startTime);
            onChangeStartTime?.Invoke();
            _tweener.Kill(false);
            Run();
        }

        public TimeTweener AddOnCompleteListener(Action onComplete)
        {
            OnComplete += onComplete;
            return this;
        }

        public TimeTweener AddOnUpdateListener(Action<TimeTweenerData> onUpdate)
        {
            OnUpdate += onUpdate;
            return this;
        }

        private Tweener _tweener;

        public void Run()
        {
            double offlineTime = GetOfflineTime();
            double duration = _duration - offlineTime;
            
            var currentTime = GetCurrentTime();
            var left = (_endTime - currentTime).TotalSeconds;
            var target = (_endTime - _startTime).TotalSeconds;

            if (duration <= 0)
            {
                TweenerData = new TimeTweenerData(0, target);
                OnUpdate?.Invoke(TweenerData);
                OnComplete?.Invoke();
               
               
                return;
            }


            _tweener = DOVirtual.Float((float)offlineTime, (float)target, (float)left,
                    progressTime =>
                    {
                        double leftTime = target - progressTime;
                        double passTime = target - leftTime;
  
                        TweenerData = new TimeTweenerData(leftTime, passTime);
                        OnUpdate?.Invoke(TweenerData);
                    })
                .SetEase(Ease.Linear)
                .SetUpdate(_ignoreTimeScale)
                .OnComplete(() => OnComplete?.Invoke());
        }

        public void Kill(bool complete)
        {
            _tweener.Kill(complete);
            if(complete)
                OnComplete?.Invoke();
            
            Listeners.ClearListeners(OnComplete);
            Listeners.ClearListeners(OnUpdate);
        }

        public double GetOfflineTime()
        {
            return  (GetCurrentTime() - _startTime).TotalSeconds;
        }

        public DateTime GetCurrentTime()
        {
            return _timeService.TimeStorageData.ReadDateTime(TimeData.CurrentTimeName);
        }
        
        public bool IsCompleted()
        {
            double offlineTime = GetOfflineTime();
            var current = _startTime.AddMilliseconds(offlineTime * 1000);
            var target = _startTime.AddMilliseconds(_duration * 1000);
            var comlete =current >= target;
            return comlete;
        }
        
        
        public struct TimeTweenerData
        {
            public double LeftTime { get; private set; }
            public double PassTime { get; private set; }
            public TimeTweenerData(double leftTime, double passTime)
            {
                LeftTime = leftTime;
                PassTime = passTime;
            }
        }
    }
}

using System;
using System.Collections;
using LittleBit.Modules.CoreModule;
using LittleBit.Modules.CoreModule.Tools;
using UnityEngine;

namespace LittleBit.Modules.TimeServiceModule
{
    public class TimeService : IService, ITimeService
    {
        private readonly IDataStorageService _dataStorageService;
        public const string TimeDataKey = "TimeData";
        public TimeData TimeStorageData { get; private set; }

        private readonly ITimeGetter _deviceTimeGetter, _serverTimeGetter, _fallbackTimeGetter;

        private readonly ICoroutineRunner _coroutineRunner;
        private readonly FirstPlayChecker _firstPlayChecker;
        private ILifecycle _appLifecycle;

        private bool _isInitialized;

        public event Action<long> OnTimeInitialized;
 
        public TimeService(IDataStorageService dataStorageService, ICoroutineRunner coroutineRunner,
            FirstPlayChecker firstPlayChecker, ILifecycle appLifecycle)
        {
            _appLifecycle = appLifecycle;
            _serverTimeGetter = new ServerTimeGetter();
            _deviceTimeGetter = new DeviceTimeGetter();

            _fallbackTimeGetter = _deviceTimeGetter;

            _firstPlayChecker = firstPlayChecker;
            _coroutineRunner = coroutineRunner;
            _dataStorageService = dataStorageService;
            _coroutineRunner.StartCoroutine(Interval(10, SyncTime));
            

            // SubscribeToAppLifecycle();

            InitTime();
        }

        private void SubscribeToAppLifecycle()
        {
            _appLifecycle.onApplicationQuit += () =>
            {
                SetLastCloseTime();

                Debug.Log("(TimeTest) Saving time on quit: " + _dataStorageService.GetData<TimeData>(TimeDataKey));
            };

            _appLifecycle.onApplicationFocus += focus =>
            {
                if (focus) return;

                SetLastCloseTime();

                Debug.Log("(TimeTest) Saving time on focus: " + _dataStorageService.GetData<TimeData>(TimeDataKey));
            };

            _appLifecycle.onApplicationPause += pause =>
            {
                if (!pause) return;

                SetLastCloseTime();

                Debug.Log("(TimeTest) Saving time on pause: " + _dataStorageService.GetData<TimeData>(TimeDataKey));
            };
        }

        private void SetLastCloseTime()
        {
            TimeStorageData.WriteDateTime(TimeData.LastCloseTimeName,
                TimeStorageData.ReadDateTime(TimeData.CurrentTimeName));

            _dataStorageService.SetData(TimeDataKey, TimeStorageData);
        }

        private void SyncTime()
        {
            GetCurrentTime(_serverTimeGetter,
                time =>
                {
                    TimeStorageData.WriteDateTime(TimeData.CurrentTimeName, time);

                    Debug.Log("(TimeTest) Time was synchronised: " +
                              TimeStorageData.ReadDateTime(TimeData.CurrentTimeName));
                });
        }

        private void InitTime()
        {
            if (_firstPlayChecker.IsFirstPlay()) System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();

            TimeStorageData = _dataStorageService.GetData<TimeData>(TimeDataKey);

            TimeStorageData.WriteDateTime(TimeData.LastCloseTimeName,
                TimeStorageData.ReadDateTime(TimeData.CurrentTimeName));

            GetCurrentTime(_deviceTimeGetter, time =>
            {
                TimeStorageData.WriteDateTime(TimeData.CurrentTimeName, time);

                TimeStorageData.WriteDateTime(TimeData.TimeOpenAppName, time);

                if (_firstPlayChecker.IsFirstPlay())
                {
                    TimeStorageData.WriteDateTime(TimeData.LastCloseTimeName, time);
                }

                _dataStorageService.SetData(TimeDataKey, TimeStorageData);

                OnTimeInitialized?.Invoke(TimeStorageData.CurrentTimeInSeconds);

                _coroutineRunner.StartCoroutine(TicksLoop());
            });
        }

        private void GetCurrentTime(ITimeGetter timeGetter, Action<DateTime> callback)
        {
            TryGetTime(timeGetter, _fallbackTimeGetter, dateTime => { callback?.Invoke(dateTime); });
        }

        private void GetTime(ITimeGetter timeGetter, Action<DateTime> callback, Action error)
        {
            _coroutineRunner.StartCoroutine(timeGetter.GetCurrentTime(dateTime => { callback?.Invoke(dateTime); },
                error));
        }

        private IEnumerator TicksLoop()
        {
            var frames = 0;
            const int framesToSave = 60;

            while (true)
            {
                if (!_isInitialized) yield return null;

                TimeStorageData.WriteDateTime(TimeData.CurrentTimeName, DateTime.UtcNow);

                _dataStorageService.SetData(TimeDataKey,
                    TimeStorageData, SaveMode.NotSave);

                frames++;

                if (frames >= framesToSave)
                {
                    frames = 0;


                    TimeStorageData.WriteDateTime(TimeData.LastCloseTimeName,
                        TimeStorageData.ReadDateTime(TimeData.CurrentTimeName));

                    _dataStorageService.SetData(TimeDataKey,
                        TimeStorageData);
                }

                yield return null;
            }
        }

        private void TryGetTime(ITimeGetter timeGetter, ITimeGetter fallbackTimeGetter, Action<DateTime> callback)
        {
            void Error()
            {
                GetTime(fallbackTimeGetter, callback, null);
            }

            GetTime(timeGetter, callback, Error);
        }

        private IEnumerator Interval(float interval, Action callback)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(interval);

                callback?.Invoke();
            }
        }
    }
}
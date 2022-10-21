using System;
using LittleBit.Modules.CoreModule;
using LittleBit.Modules.CoreModule.Tools;

namespace LittleBit.Modules.TimeServiceModule
{
    public class TimeData : Data
    {
        public SerializableDictionary<string, string> _dateTimes;

        public const string CurrentTimeName = "CurrentTime";
        public const string LastCloseTimeName = "LastCloseTime";
        public const string TimeOpenAppName = "TimeOpenApp";

        public long CurrentTimeInSeconds
        {
            get { return ReadDateTime(CurrentTimeName).TotalSeconds(); }
        }

        public long LastCloseTimeInSeconds
        {
            get { return ReadDateTime(LastCloseTimeName).TotalSeconds(); }
        }

        public long TimeOpenAppInSeconds
        {
            get { return ReadDateTime(TimeOpenAppName).TotalSeconds(); }
        }


        public TimeData(DateTime timeOpenApp, DateTime currentTime)
        {
            WriteDateTime(TimeOpenAppName, timeOpenApp);
            WriteDateTime(CurrentTimeName, currentTime);
        }

        public void WriteDateTime(string name, DateTime dateTime)
        {
            var binaryString = dateTime.ToBinary().ToString();

            if (!_dateTimes.ContainsKey(name))
            {
                _dateTimes.Add(name, binaryString);
                return;
            }

            _dateTimes[name] = binaryString;
        }

        public DateTime ReadDateTime(string name)
        {
            return !_dateTimes.ContainsKey(name) ? DateTime.UtcNow : ConvertStringToDateTime(_dateTimes[name]);
        }

        public TimeData()
        {
            _dateTimes = new SerializableDictionary<string, string>();
        }

        public static DateTime ConvertStringToDateTime(string binary)
        {
            return DateTime.FromBinary(Convert.ToInt64(binary));
        }
    }

    public static class TimeExtension
    {
        public static long TotalSeconds(this DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
    }
}
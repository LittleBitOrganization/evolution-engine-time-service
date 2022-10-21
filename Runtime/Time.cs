using System;

namespace LittleBit.Modules.TimeServiceModule
{    
    [Serializable]
    public class Time
    {
        public bool IsNull => string.IsNullOrEmpty(Value);
        public string Value;

        public Time() => Value = "";
        public Time(string value) => Value = value;
        public Time(long time) => Value = time.ToString();
        public Time(DateTime dataTime) => Value = dataTime.ToBinary().ToString();

        public void SetNewDataTime(DateTime dateTime)
        {
            Value = dateTime.ToBinary().ToString();
        }
    }

}
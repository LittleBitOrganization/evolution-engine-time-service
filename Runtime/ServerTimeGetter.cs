using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace LittleBit.Modules.TimeServiceModule
{
    public class ServerTimeGetter : ITimeGetter
    {
        private const string URL = "http://worldclockapi.com/api/json/utc/now";

        public IEnumerator GetCurrentTime(Action<DateTime> callback, Action error)
        {
            using var request = UnityWebRequest.Get(URL);

            var timeBeforeRequest = TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
            
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                error?.Invoke();
                yield break;
            }
            
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
            
            var timeAfterRequest = TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
            var requestDuration = timeAfterRequest - timeBeforeRequest;
            
            var serverTime = DateTime.FromFileTimeUtc(long.Parse(json["currentFileTime"].ToString()));
            
            callback?.Invoke(serverTime - requestDuration);
        }
    }
}
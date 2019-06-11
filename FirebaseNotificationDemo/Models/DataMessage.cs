using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseNotificationDemo.Models
{
    public class DataMessage
    {
        [JsonProperty("registration_ids")]
        public List<string> RegistrationIds { get; set; }
        [JsonProperty("data")]
        public dynamic Data { get; set; }
        public Notification notification { get; set; }

    }
    public class Notification
    {
        public string title { get; set; }
        public string text { get; set; }
    }
}

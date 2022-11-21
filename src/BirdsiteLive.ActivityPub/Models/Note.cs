using BirdsiteLive.ActivityPub.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BirdsiteLive.ActivityPub.Models
{
    public class Note
    {
        [JsonProperty("@context")]
        [JsonConverter(typeof(ContextArrayConverter))]
        public object[] context { get; set; } = Activity.DefaultContext;

        public string id { get; set; }
        public string type { get; } = "Note";
        public string summary { get; set; }
        public string inReplyTo { get; set; }
        public string published { get; set; }
        public string url { get; set; }
        public string attributedTo { get; set; }
        public string[] to { get; set; }
        public string[] cc { get; set; }
        public bool sensitive { get; set; }
        //public string conversation { get; set; }
        public string content { get; set; }
        //public Dictionary<string,string> contentMap { get; set; }
        public Attachment[] attachment { get; set; }
        public Tag[] tag { get; set; }
        //public Dictionary<string, string> replies;

        public string quoteUrl { get; set; }
    }
}
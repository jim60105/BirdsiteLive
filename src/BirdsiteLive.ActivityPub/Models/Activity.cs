using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace BirdsiteLive.ActivityPub
{
    public class Activity
    {
        [Newtonsoft.Json.JsonIgnore]
        public static readonly object[] DefaultContext = new object[] {
            "https://www.w3.org/ns/activitystreams",
            "https://w3id.org/security/v1",
            new Dictionary<string, string>
            {
                { "Emoji", "toot:Emoji" },
                { "Hashtag", "as:Hashtag" },
                { "PropertyValue", "schema:PropertyValue" },
                { "value", "schema:value" },
                { "sensitive", "as:sensitive" },
                { "quoteUrl", "as:quoteUrl" },

                { "schema", "http://schema.org#" },
                { "toot", "https://joinmastodon.org/ns#" }
            }
        };

        [JsonProperty("@context")]
        public object context { get; set; } = DefaultContext;
        public string id { get; set; }
        public string type { get; set; }
        public string actor { get; set; }

        //[JsonProperty("object")]
        //public string apObject { get; set; }
    }
}
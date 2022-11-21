using BirdsiteLive.ActivityPub.Converters;
using Newtonsoft.Json;

namespace BirdsiteLive.ActivityPub.Models
{
    public class Followers
    {
        [JsonProperty("@context")]
        [JsonConverter(typeof(ContextArrayConverter))]
        public object[] context { get; set; } = Activity.DefaultContext;

        public string id { get; set; }
        public string type { get; set; } = "OrderedCollection";
    }
}
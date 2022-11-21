using System;

namespace BirdsiteLive.ActivityPub.Models
{
    public class Tag {
        public TagResource icon { get; set; } = null;
        public string id { get; set; }
        public string type { get; set; } //Hashtag
        public string href { get; set; } //https://mastodon.social/tags/app
        public string name { get; set; } //#app
        public DateTime updated { get; set; } = default(DateTime);
    }

    public class TagResource
    {
        public string type { get; set; }
        public string url { get; set; }
    }
}
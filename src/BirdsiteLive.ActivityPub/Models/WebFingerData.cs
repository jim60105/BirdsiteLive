using System;
using System.Collections.Generic;
using System.Text;

namespace BirdsiteLive.ActivityPub.Models
{
    public class WebFingerData
    {
        public List<string> aliases { get; set; }

        public List<WebFingerLink> links { get; set; }

        public string subject { get; set; }
    }

    public class WebFingerLink
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string type { get; set; }
        public string template { get; set; }
    }
}

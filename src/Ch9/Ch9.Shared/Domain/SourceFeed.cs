using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9.Domain
{
    [Windows.UI.Xaml.Data.Bindable]
    public class SourceFeed
    {
        public SourceFeed(string feedUrl, string name = null, Uri image = null)
        {
            FeedUrl = feedUrl;
            Name = name;
            ThumbnailUrl = image ?? new Uri("https://channel9.msdn.com/assets/images/nineguy-512-bw.png");
        }

        public string FeedUrl { get; set; }

        public string Name { get; set; }

        public Uri ThumbnailUrl { get; set; }
    }
}

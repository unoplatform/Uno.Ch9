using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9.Domain
{
    [Windows.UI.Xaml.Data.Bindable]
    public class SourceFeed
    {
        public SourceFeed(string url, string name = null, Uri image = null)
        {
            Url = url;
            Name = name;
            Image = image != null ? image : new Uri("https://channel9.msdn.com/assets/images/nineguy-512-bw.png");
        }

        public string Url { get; set; }

        public string Name { get; set; }

        public Uri Image { get; set; }
    }
}

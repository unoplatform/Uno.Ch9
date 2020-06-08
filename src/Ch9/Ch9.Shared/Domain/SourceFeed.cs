using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9.Domain
{
    public class SourceFeed
    {
        public SourceFeed(string url, string name = null)
        {
            Url = url;
            Name = name;
        }

        public string Url { get; set; }
        
        public string Name { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9
{
	public class SourceFeed
	{
		public SourceFeed(string url, string show = null)
		{
			Url = url;
			Show = show;
		}

		public string Url { get; set; }

		public string Show { get; set; }
	}
}

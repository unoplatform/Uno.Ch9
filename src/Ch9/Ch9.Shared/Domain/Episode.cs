using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9
{
	[Windows.UI.Xaml.Data.Bindable]
	public class Episode 
	{
		public string Title { get; set; }

		public string Summary { get; set; }

		public string Show { get; set; }

		public string[] Categories { get; set; }

		public TimeSpan Duration { get; set; }

		public DateTimeOffset Date { get; set; }

		public Uri ImageUri { get; set; }

		public Uri EpisodeUri { get; set; }

		public Uri VideoUri { get; set; }

		public string FormattedDate =>
			Date.Year == DateTime.Now.Year 
				? Date.ToString("MMMM dd") 
				: Date.ToString("MMMM dd yyyy");

		public string FormattedDuration => Duration.ToString("mm':'ss");
	}
}

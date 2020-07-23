using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ch9.Domain;
using Refit;

namespace Ch9
{
	public interface IShowFeedEndpoint
	{
		[Get("/rssfeeds")]
		Task<IEnumerable<SourceFeed>> GetFeeds();
	}
}

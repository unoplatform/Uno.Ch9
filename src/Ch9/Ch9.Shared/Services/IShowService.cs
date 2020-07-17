using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ch9.Domain;

namespace Ch9
{
	public interface IShowService
	{
		/// <summary>
		/// Gets a collection of hardcoded show sourcefeeds.
		/// </summary>
		/// <returns>Collection of sourceFeed</returns>
		IEnumerable<SourceFeed> GetFallbackShowFeeds();

		/// <summary>
		/// Gets a show.
		/// </summary>
		/// <returns>Show</returns>
		Task<Show> GetShow(SourceFeed sourceFeed = null);

		/// <summary>
		/// Gets a collection of show sourcefeeds.
		/// </summary>
		/// <returns>Collection of sourceFeed</returns>
		Task<IEnumerable<SourceFeed>> GetShowFeeds();
	}
}

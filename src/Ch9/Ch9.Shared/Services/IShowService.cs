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
		/// Gets a collection of shows.
		/// </summary>
		/// <returns>Collection of shows.</returns>
		IEnumerable<SourceFeed> GetShowFeeds();

		/// <summary>
		/// Gets a show.
		/// </summary>
		/// <returns>Show</returns>
		Task<Show> GetShow(SourceFeed sourceFeed = null);
    }
}

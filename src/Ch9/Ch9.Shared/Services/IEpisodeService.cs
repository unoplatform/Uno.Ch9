using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ch9.Domain;

namespace Ch9
{
	public interface IEpisodeService
	{
		/// <summary>
		/// Gets a collection of recent posts.
		/// </summary>
		/// <returns>Collection of recent posts</returns>
		Task<Episode[]> GetRecentEpisodes(SourceFeed showFeed = null);
    }
}

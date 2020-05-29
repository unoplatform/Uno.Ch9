using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Ch9
{
    public class EpisodeService : IEpisodeService
    {
        private const string YahooNamespace = "http://search.yahoo.com/mrss/";
        private const string ITunesNamespace = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        public const string Channel9 = "https://s.ch9.ms/feeds/rss";

        public Task<Episode[]> GetRecentEpisodes(Show show = null)
        {
            string url;

            url = show == null ? Channel9 : show.Url;

            return Task.Run(() =>
            {
                var episodes = new List<Episode>();

                var rssFeed = GetRssFeed(url);

                var feedPosts = rssFeed
                    .Items
                    .Select(i => CreatePost(i, show?.Name))
                    .ToArray();

                episodes.AddRange(feedPosts);

                var comparer = new PostEqualityComparer();

                return episodes
                    .Distinct(comparer)
                    .OrderByDescending(p => p.Date)
                    .ToArray();
            });
        }

        private SyndicationFeed GetRssFeed(string url)
        {
            using (var reader = XmlReader.Create(url))
            {
                return SyndicationFeed.Load(reader);
            }
        }

        private Episode CreatePost(SyndicationItem item, string showName)
        {
            return new Episode
            {
                Title = GetTitle(item),
                Show = showName ?? GetShow(item),
                Summary = GetSummary(item),
                Date = item.PublishDate,
                Categories = GetCategories(item).ToArray(),
                ImageUri = GetThumbnailUri(item),
                PostUri = GetPostUri(item),
                VideoUri = GetVideoUri(item),
                Duration = GetDuration(item)
            };
        }

        private string GetTitle(SyndicationItem item)
        {
            var title = item.Title.Text.Split("|").FirstOrDefault();

            return title?.Trim();
        }

        private string GetShow(SyndicationItem item)
        {
            var show = item.Title.Text.Split("|").ElementAt(1);

            return show?.Trim();
        }

        private string GetSummary(SyndicationItem item)
        {
            return item.ElementExtensions.ReadElementExtensions<XElement>("summary", ITunesNamespace).Single().Value;
        }

        private string[] GetCategories(SyndicationItem item)
        {
            return item.Categories.Select(c => c.Name).ToArray();
        }

        private TimeSpan GetDuration(SyndicationItem item)
        {
            var duration = item.ElementExtensions.ReadElementExtensions<XElement>("duration", ITunesNamespace).Single().Value;

            return TimeSpan.FromSeconds(Int32.Parse(duration));
        }

        private const int ThumbnailPreferredWidth = 512;
        private static readonly Regex ThumbnailRegex = new Regex("(.*)_(.*)\\.(.*)");
        private Uri GetThumbnailUri(SyndicationItem item)
        {
            var thumbnails = item.ElementExtensions.ReadElementExtensions<XElement>("thumbnail", YahooNamespace);

            // We take the last thumbnail as it's the one with the highest resolution.
            var thumbnail = thumbnails.LastOrDefault()?.Attribute("url").Value;

            // Because not all resolutions are reported in the items,
            // we build the thumbnail uri with a preferred width that exists.
            // If it's not a match, we'll just use the last thumbnail.
            var match = ThumbnailRegex.Match(thumbnail);
            if (match.Success && match.Groups.Count == 4)
            {
                var route = match.Groups[1].Value;
                var extension = match.Groups[3].Value;

                thumbnail = $"{route}_{ThumbnailPreferredWidth}.{extension}";
            }

            return thumbnail != null
                ? new Uri(thumbnail)
                : null;
        }

        private Uri GetPostUri(SyndicationItem item)
        {
            return item.Links.SingleOrDefault(s => s.RelationshipType == "alternate").Uri;
        }

        private Uri GetVideoUri(SyndicationItem item)
        {
            return item.Links.SingleOrDefault(s => s.MediaType == "video/mp4").Uri;
        }

        private class PostEqualityComparer : IEqualityComparer<Episode>
        {
            public bool Equals(Episode x, Episode y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                if (x.Title == y.Title) return true;

                return false;
            }

            public int GetHashCode(Episode obj) => obj.Title.GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Ch9.Domain;

namespace Ch9
{
    public class ShowService : IShowService
    {
        private const string YahooNamespace = "http://search.yahoo.com/mrss/";
        private const string ITunesNamespace = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        private static readonly SourceFeed _channel9Feed = new SourceFeed("https://s.ch9.ms/feeds/rss", "Channel 9");

        private readonly IDictionary<string, Show> _cache = new Dictionary<string, Show>();

        /// <inheritdoc/>
        public IEnumerable<SourceFeed> GetShowFeeds()
        {
            return new List<SourceFeed>
            {
                new SourceFeed("https://s.ch9.ms/Shows/Visual-Studio-Toolbox/feed", "Visual Studio Toolbox"),
                new SourceFeed("https://s.ch9.ms/Shows/Partly-Cloudy/feed", "Party Cloudy"),
                new SourceFeed("https://s.ch9.ms/Shows/Azure-Friday/feed", "Azure Friday"),
                new SourceFeed("https://s.ch9.ms/Shows/XamarinShow/feed", "Xamarin Show"),
                new SourceFeed("https://s.ch9.ms/Shows/This+Week+On+Channel+9/feed", "This Week On Channel9"),
                new SourceFeed("https://s.ch9.ms/Blogs/One-Dev-Minute/feed", "On Dev Minute"),
                new SourceFeed("https://s.ch9.ms/Series/Intro-to-Visual-Studio-for-Mac/feed", "Intro To Visual Studio For Mac"),
                new SourceFeed("https://s.ch9.ms/Shows/AI-Show/feed", "AI Show"),
                new SourceFeed("https://s.ch9.ms/Series/C-Advanced/feed", "C Advanced"),
                new SourceFeed("https://s.ch9.ms/Series/CSharp-101/feed", "CSharp 101"),
                new SourceFeed("https://s.ch9.ms/Series/NET-Core-101/feed", "NetCore 101"),
                new SourceFeed("https://s.ch9.ms/Series/Intro-to-Visual-Studio/feed", "Intro To Visual Studio"),
                new SourceFeed("https://s.ch9.ms/Series/Intro-to-Python-Development/feed", "Intro To Python Development"),
                new SourceFeed("https://s.ch9.ms/Shows/Less-Code-More-Power/feed", "Less Code More Power"),
                new SourceFeed("https://s.ch9.ms/Shows/CodeStories/feed", "Code Stories"),
                new SourceFeed("https://s.ch9.ms/Shows/Careers-Behind-the-Code/feed", "Careers Behind The Code"),
                new SourceFeed("https://s.ch9.ms/Shows/On-NET/feed", "On Net"),
            };
        }

        /// <inheritdoc/>
        public Task<Show> GetShow(SourceFeed sourceFeed = null)
        {
            var url = sourceFeed != null ? sourceFeed.Url : _channel9Feed.Url;

            if (_cache.TryGetValue(url, out var cachedShow))
            {
                return Task.FromResult(cachedShow);
            }

            var rssFeed = GetRssFeed(url);

            var show = new Show()
            {
                Description = rssFeed.Description.Text,
                Image = rssFeed.ImageUrl,
                Name = sourceFeed?.Name
            };

            show.Episodes = GetEpisodes(sourceFeed, rssFeed);

            _cache.Add(url, show);

            return Task.FromResult(show);
        }

        private IEnumerable<Episode> GetEpisodes(SourceFeed sourceFeed, SyndicationFeed rssFeed)
        {
            var episodes = new List<Episode>();

            var feedPosts = rssFeed
                .Items
                .Select(i => CreatePost(i, sourceFeed))
                .ToArray();

            episodes.AddRange(feedPosts);

            var comparer = new EpisodeEqualityComparer();

            return episodes
                .Distinct(comparer)
                .OrderByDescending(p => p.Date)
                .ToArray();
        }

        private SyndicationFeed GetRssFeed(string url)
        {
            using (var reader = XmlReader.Create(url))
            {
               return SyndicationFeed.Load(reader);
            }
        }

        private Episode CreatePost(SyndicationItem item, SourceFeed sourceFeed)
        {
            return new Episode
            {
                Title = GetTitle(item),
                Show =  GetShow(item, sourceFeed),
                Summary = GetSummary(item),
                Date = item.PublishDate,
                Categories = GetCategories(item).ToArray(),
                ImageUri = GetThumbnailUri(item),
                EpisodeUri = GetPostUri(item),
                VideoUri = GetVideoUri(item),
                Duration = GetDuration(item)
            };
        }

        private string GetTitle(SyndicationItem item)
        {
            var title = item.Title.Text.Split("|").FirstOrDefault();

            return title?.Trim();
        }

        private string GetShow(SyndicationItem item, SourceFeed sourceFeed)
        {
            if (sourceFeed != null)
            {
                return sourceFeed.Name;
            }

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

        private class EpisodeEqualityComparer : IEqualityComparer<Episode>
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Ch9.Domain;
using Ch9.Services;
using GalaSoft.MvvmLight.Ioc;

namespace Ch9
{
    public class EpisodeService : IEpisodeService
    {
        private const string YahooNamespace = "http://search.yahoo.com/mrss/";
        private const string ITunesNamespace = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        private const string Channel9 = "https://s.ch9.ms/feeds/rss";

        private readonly IShowService _showService;

        public Task<Episode[]> GetRecentEpisodes(SourceFeed showFeed = null)
        {
            string url;

            url = showFeed == null ? Channel9 : showFeed.Url;

            return Task.Run(() =>
            {
                var episodes = new List<Episode>();

                var rssFeed = GetRssFeed(url);

                _showService.SetCurrentShow(rssFeed.Description.Text, rssFeed.ImageUrl, rssFeed.Title.Text);

                var feedPosts = rssFeed
                    .Items
                    .Select(i => CreatePost(i))
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

        private Episode CreatePost(SyndicationItem item)
        {
            return new Episode
            {
                Title = GetTitle(item),
                Show =  GetShow(item),
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
            return _showService.GetCurrentShow().Name;
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

        public EpisodeService()
        {
            _showService = SimpleIoc.Default.GetInstance<IShowService>();
        }

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

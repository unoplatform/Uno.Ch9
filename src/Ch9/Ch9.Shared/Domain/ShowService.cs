using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Networking.Connectivity;
using Ch9.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Uno.Extensions;
using Uno.Logging;
using System.Globalization;

namespace Ch9
{
    public class ShowService : IShowService
    {
        private const string YahooNamespace = "http://search.yahoo.com/mrss/";
        private const string ITunesNamespace = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        // private static readonly SourceFeed _channel9Feed = new SourceFeed("https://s.ch9.ms/feeds/rss", "Channel 9");
        private static readonly SourceFeed _channel9Feed = new SourceFeed("https://learntvpublicschedule.azureedge.net/public/schedule.json", "Microsoft Learn Shows (ex-Channel 9)");

        private readonly IDictionary<string, Show> _cache = new Dictionary<string, Show>();

        private readonly HttpClient _httpClient;

        public ShowService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc/>
        private IEnumerable<SourceFeed> GetFallbackShowFeeds()
        {
            return new List<SourceFeed>
            {
                new SourceFeed("https://s.ch9.ms/Shows/Visual-Studio-Toolbox/feed", "Visual Studio Toolbox", new Uri("https://f.ch9.ms/thumbnail/76a9b7c3-0474-4194-83ac-84a4e0919cc8.png")),
                new SourceFeed("https://s.ch9.ms/Shows/Partly-Cloudy/feed", "Party Cloudy", new Uri("https://f.ch9.ms/thumbnail/b752fb27-198a-4a14-a7a9-f640801adb65.jpg")),
                new SourceFeed("https://s.ch9.ms/Shows/Azure-Friday/feed", "Azure Friday", new Uri("https://f.ch9.ms/thumbnail/3cf59759-da96-47c7-b703-eb21e538dd09.png")),
                new SourceFeed("https://s.ch9.ms/Shows/XamarinShow/feed", "Xamarin Show", new Uri("https://f.ch9.ms/thumbnail/8f2c0861-7314-44da-b0ee-f20b13906381.png")),
                new SourceFeed("https://s.ch9.ms/Shows/This+Week+On+Channel+9/feed", "This Week On Channel9", new Uri("https://f.ch9.ms/thumbnail/65cbe7c6-31ba-4b5e-97bb-ca0a9ecd6203.png")),
                new SourceFeed("https://s.ch9.ms/Blogs/One-Dev-Minute/feed", "On Dev Minute", new Uri("https://f.ch9.ms/thumbnail/79b44705-876c-4713-b135-cd3b20aae6bb.png")),
                new SourceFeed("https://s.ch9.ms/Series/Intro-to-Visual-Studio-for-Mac/feed", "Intro To Visual Studio For Mac", new Uri("https://f.ch9.ms/thumbnail/33a2141b-0245-4ffc-b2d1-65142895db9b.jpg")),
                new SourceFeed("https://s.ch9.ms/Shows/AI-Show/feed", "AI Show", new Uri("https://f.ch9.ms/thumbnail/b36b7fb8-5a33-4e87-b5ba-4c3ed3537a23.png")),
                new SourceFeed("https://s.ch9.ms/Series/C-Advanced/feed", "C# Advanced", new Uri("https://f.ch9.ms/thumbnail/368c6b87-5a56-40fe-8291-365c3785aada.png")),
                new SourceFeed("https://s.ch9.ms/Series/CSharp-101/feed", "CSharp 101", new Uri("https://f.ch9.ms/thumbnail/2888a106-8416-49fe-b4f7-200306758639.jpg")),
                new SourceFeed("https://s.ch9.ms/Series/NET-Core-101/feed", "NetCore 101", new Uri("https://f.ch9.ms/thumbnail/06acd3e0-2207-44d3-87c6-3676e0e83215.jpg")),
                new SourceFeed("https://s.ch9.ms/Series/Intro-to-Visual-Studio/feed", "Intro To Visual Studio", new Uri("https://f.ch9.ms/thumbnail/7d0b6c57-a4f8-4c25-8be5-822b0099ec59.jpg")),
                new SourceFeed("https://s.ch9.ms/Series/Intro-to-Python-Development/feed", "Intro To Python Development", new Uri("https://f.ch9.ms/thumbnail/9df8280a-cbb8-451f-b41b-160e99c855c9.png")),
                new SourceFeed("https://s.ch9.ms/Shows/Less-Code-More-Power/feed", "Less Code More Power", new Uri("https://f.ch9.ms/thumbnail/c89c39ac-92ac-416d-8f7a-a58ffdb867b3.png")),
                new SourceFeed("https://s.ch9.ms/Shows/CodeStories/feed", "Code Stories", new Uri("https://f.ch9.ms/thumbnail/f5d0cb49-d3cc-4428-b20a-b3909a98e691.jpg")),
                new SourceFeed("https://s.ch9.ms/Shows/Careers-Behind-the-Code/feed", "Careers Behind The Code", new Uri("https://f.ch9.ms/thumbnail/97eb6439-bc25-4552-9dba-708c6f02222a.jpg")),
                new SourceFeed("https://s.ch9.ms/Shows/On-NET/feed", "On Net", new Uri("https://f.ch9.ms/thumbnail/c96889ca-5f16-4bde-ab54-be4f6ecdee7a.png")),
            };
        }

        public async Task<IEnumerable<SourceFeed>> GetShowFeeds()
        {
            // If any exception occurs, fallback to the list of hardcoded shows
            try
            {
                var response = await _httpClient.GetStringAsync("api/rssfeeds");

                return JsonConvert.DeserializeObject<SourceFeed[]>(response);
            }
            catch (Exception e)
            {
                if (!IsInternetAvailable()) throw;

                this.Log().Warn("Couldn't load the shows. Fallbacking on the default shows.", e);
                return GetFallbackShowFeeds();
            }

            bool IsInternetAvailable()
            {
                var profile = NetworkInformation.GetInternetConnectionProfile();
                var level = profile?.GetNetworkConnectivityLevel();
                return level == NetworkConnectivityLevel.InternetAccess;
            }
        } 

        /// <inheritdoc/>
        public async Task<Show> GetShow(SourceFeed sourceFeed = null)
        {
            var url = sourceFeed != null ? sourceFeed.FeedUrl : _channel9Feed.FeedUrl;

            if (_cache.TryGetValue(url, out var cachedShow))
            {
                return cachedShow;
            }

            var rssFeed = await GetRssFeed(url);

            var show = new Show()
            {
                Description = rssFeed.Description.Text,
                Image = sourceFeed?.ThumbnailUrl,
                Name = sourceFeed?.Name
            };

            show.Episodes = GetEpisodes(sourceFeed, rssFeed);

            _cache.Add(url, show);

            return show;
        }

        private IEnumerable<Episode> GetEpisodes(SourceFeed sourceFeed, SyndicationFeed rssFeed)
        {
            var episodes = new List<Episode>();

            var feedEpisodes = rssFeed
                .Items
                .Select(i => CreateEpisode(i, sourceFeed))
                .ToArray();

            episodes.AddRange(feedEpisodes);

            var comparer = new EpisodeEqualityComparer();

            return episodes
                .Distinct(comparer)
                .OrderByDescending(p => p.Date)
                .ToArray();
        }

        private async Task<SyndicationFeed> GetRssFeed(string url)
        {
            using (var reader = await HttpUtility.GetJsonReader(url))
            {
                var feedItems = await ReadFeed(reader);
                SyndicationFeed feed = new SyndicationFeed(feedItems);
                feed.Description = SyndicationContent.CreatePlaintextContent("Microsoft Learn TV");
                return feed;
            }
        }

        private static async Task<List<SyndicationItem>> ReadFeed(JsonTextReader reader)
        {
            List<SyndicationItem> feedItems = new List<SyndicationItem>();
            SyndicationItem currentItem = null;
            String currentProperty = null;
            DateTimeOffset? startTime = null;
            DateTimeOffset? endTime = null;
            Boolean? isLive = null;
            while (await reader.ReadAsync())
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        currentItem = new SyndicationItem();
                        break;
                    case JsonToken.EndObject:
                        CompleteFeedItem(currentItem, startTime, endTime, isLive);

                        startTime = null;
                        endTime = null;
                        isLive = null;

                        feedItems.Add(currentItem);
                        break;
                    case JsonToken.PropertyName:
                        currentProperty = reader.Value.ToString();
                        break;
                    case JsonToken.Integer:
                        if(currentProperty == "pubble_id")
                        {
                            currentItem.Id = reader.Value.ToString();
                        }
                        break;
                    case JsonToken.String:
                        switch (currentProperty)
                        {
                            case "title": {
                                    currentItem.Title = SyndicationContent.CreatePlaintextContent($"|{reader.Value.ToString()}");
                                    break;
                                }
                            case "description": {
                                    currentItem.Summary = SyndicationContent.CreatePlaintextContent(reader.Value.ToString());
                                    break;
                                }
                            case "externalurl": {
                                    currentItem.BaseUri = new Uri(reader.Value.ToString());
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    case JsonToken.Boolean:
                        if (currentProperty == "islive")
                        {
                            isLive = (Boolean)reader.Value;
                        }
                        break;
                    case JsonToken.Date:
                        switch (currentProperty)
                        {
                            case "start_time":
                                {
                                    startTime = DateTimeOffset.Parse(reader.Value.ToString());
                                    currentItem.PublishDate = startTime.Value;
                                    break;
                                }
                            case "end_time":
                                {
                                    endTime = DateTimeOffset.Parse(reader.Value.ToString());
                                    break;
                                }
                            default:
                                break;
                        }
                        break;
                    // Commented out because not necessary, but useful for future improvements
                    //case JsonToken.Undefined:
                    //	break;
                    //case JsonToken.None:
                    //	break;
                    //case JsonToken.Null:
                    //    break;
                    //case JsonToken.StartArray:
                    //	break;
                    //case JsonToken.EndArray:
                    //    break;
                    //case JsonToken.StartConstructor:
                    //	break;
                    //case JsonToken.Float:
                    //	break;
                    //case JsonToken.Comment:
                    //	break;
                    //case JsonToken.Raw:
                    //	break;
                    //case JsonToken.EndConstructor:
                    //	break;
                    //case JsonToken.Bytes:
                    //	break;
                    default:
                        break;
                }
            }

            return feedItems;
        }

        private static void CompleteFeedItem(SyndicationItem currentItem, DateTimeOffset? startTime, DateTimeOffset? endTime, bool? isLive)
        {
            String details = "";
            if (isLive != null)
            {
                details += $"Is Live Now: {isLive.Value}<br/>\\n";
            }
            if (startTime != null)
            {
                details += $"Starts At: {startTime.Value.ToString(CultureInfo.CurrentCulture)}<br/>\\n";
            }
            if (endTime != null)
            {
                details += $"Ends At: {endTime.Value.ToString(CultureInfo.CurrentCulture)}<br/>\\n";
            }

            if (!String.IsNullOrWhiteSpace(details))
            {
                currentItem.Content = SyndicationContent.CreateHtmlContent(details);
            }
        }

        private Episode CreateEpisode(SyndicationItem item, SourceFeed sourceFeed)
        {
            return new Episode
            {
                Title = GetTitle(item),
                Show = GetShow(item, sourceFeed),
                Summary = GetSummary(item),
                Date = item.PublishDate,
                Categories = GetCategories(item).ToArray(),
                ImageUri = GetThumbnailUri(item),
                EpisodeUri = GetEpisodeUri(item),
                VideoUri = GetVideoUri(item),
                Duration = GetDuration(item)
            };
        }

        private string GetTitle(SyndicationItem item)
        {
            var title = item.Title.Text.Split(new string[] {"|"}, StringSplitOptions.None).FirstOrDefault();

            return title?.Trim();
        }

        private string GetShow(SyndicationItem item, SourceFeed sourceFeed)
        {
            if (sourceFeed != null)
            {
                return sourceFeed.Name;
            }

            var show = item.Title.Text.Split(new string[] { "|" }, StringSplitOptions.None).ElementAt(1);

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

        private Uri GetEpisodeUri(SyndicationItem item)
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

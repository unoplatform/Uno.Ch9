using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ch9.Domain;

namespace Ch9.Services
{
    public class ShowService : IShowService
    {
        public Show CurrentShow { get; set; }

        public ShowService()
        {
            CurrentShow = new Show();
        }

        public ICollection<SourceFeed> GetShowFeeds()
        {
            var showFeedList = new List<SourceFeed>
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

            return showFeedList.OrderBy(s => s.Name).ToList();
        }

        public Show GetCurrentShow()
        {
            return CurrentShow;
        }

        public void SetCurrentShow(string description, Uri imageUrl, string title)
        {
            CurrentShow.Name = title.Split("-").FirstOrDefault();
            CurrentShow.Description = description;
            CurrentShow.Image = imageUrl;
        }
    }
}

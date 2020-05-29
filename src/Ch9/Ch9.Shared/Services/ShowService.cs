using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9.Services
{
    public class ShowService : IShowService
    {
        public ICollection<Show> GetShows()
        {
            return new List<Show>
            {
                new Show("https://s.ch9.ms/Shows/Visual-Studio-Toolbox/feed", "Visual studio Box"),
                new Show("https://s.ch9.ms/Shows/Partly-Cloudy/feed", "Party Cloudy"),
                new Show("https://s.ch9.ms/Shows/Azure-Friday/feed", "Azure Friday"),
                new Show("https://s.ch9.ms/Shows/XamarinShow/feed", "Xamarin Show"),
                new Show("https://s.ch9.ms/Shows/This+Week+On+Channel+9/feed", "This Week On Channel9"),
                new Show("https://s.ch9.ms/Blogs/One-Dev-Minute/feed", "On Dev Minute"),
                new Show("https://s.ch9.ms/Series/Intro-to-Visual-Studio-for-Mac/feed", "Intro To Visual Studio For Mac"),
                new Show("https://s.ch9.ms/Series/C-Advanced/feed", "C Advanced"),
                new Show("https://s.ch9.ms/Series/CSharp-101/feed", "CSharp 101"),
                new Show("https://s.ch9.ms/Series/NET-Core-101/feed", "NetCore 101"),
                new Show("https://s.ch9.ms/Series/Intro-to-Visual-Studio/feed", "Intro To Visual Studio"),
                new Show("https://s.ch9.ms/Series/Intro-to-Python-Development/feed", "Intro To Python Development"),
                new Show("https://s.ch9.ms/Shows/Less-Code-More-Power/feed", "Less Code More Power"),
                new Show("https://s.ch9.ms/Shows/CodeStories/feed", "Code Stories"),
                new Show("https://s.ch9.ms/Shows/Careers-Behind-the-Code/feed", "Careers Behind The Code"),
                new Show("https://s.ch9.ms/Shows/On-NET/feed", "On Net"),
            };
        }
    }
}

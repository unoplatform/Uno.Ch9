using System;
using System.Collections.Generic;
using System.Text;
using Ch9.Domain;

namespace Ch9.Services
{
    public interface IShowService
    {
        ICollection<SourceFeed> GetShowFeeds();

        Show GetCurrentShow();

        void SetCurrentShow(string description, Uri imageUrl, string title);
    }
}

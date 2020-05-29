using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9.Services
{
    public interface IShowService
    {
        ICollection<Show> GetShows();
    }
}

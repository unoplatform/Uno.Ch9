using System;
using System.Collections.Generic;
using System.Text;

namespace Ch9
{
    [Windows.UI.Xaml.Data.Bindable]
    public class Show
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Uri Image { get; set; }

        public IEnumerable<Episode> Episodes { get; set; }
    }
}

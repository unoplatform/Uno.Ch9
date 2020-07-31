using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Ch9.ViewModels
{
	[Windows.UI.Xaml.Data.Bindable]
	public class EpisodeViewModel
	{
		public EpisodeViewModel(ViewModelBase parent, Episode episode)
		{
			Parent = parent;
			Episode = episode;
			VideoUri = episode.VideoUri;
#if !__WASM__
			VideoSource = MediaSource.CreateFromUri(episode.VideoUri);
#endif
		}

		public ViewModelBase Parent { get; }

		public Episode Episode { get; }
		
		public IMediaPlaybackSource VideoSource { get; }

		public Uri VideoUri { get; }
	}
}

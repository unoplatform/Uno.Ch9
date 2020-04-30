using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Ch9.ViewModels
{
	[Windows.UI.Xaml.Data.Bindable]
	public class PostViewModel
	{
		public PostViewModel(ViewModelBase parent, Post post)
		{
			Parent = parent;
			Post = post;
			VideoSource = MediaSource.CreateFromUri(post.VideoUri);
		}

		public ViewModelBase Parent { get; }

		public Post Post { get; }
		
		public IMediaPlaybackSource VideoSource { get; }
	}
}

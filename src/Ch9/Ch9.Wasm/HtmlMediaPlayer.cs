using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Uno.Foundation;
using Uno.UI.Runtime.WebAssembly;

namespace Ch9.Wasm
{
	[HtmlElement("video")]
	public sealed partial class HtmlMediaPlayer : Border
	{
		public HtmlMediaPlayer()
		{
			Background = new SolidColorBrush(Colors.Transparent);
		}

		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
			"Source", typeof(string), typeof(HtmlMediaPlayer), new PropertyMetadata(default(string), OnSourceChanged));

		public string Source
		{
			get => (string)GetValue(SourceProperty);
			set => SetValue(SourceProperty, value);
		}

		private static async void OnSourceChanged(DependencyObject dependencyobject, DependencyPropertyChangedEventArgs args)
		{
			if (dependencyobject is HtmlMediaPlayer player)
			{
				var encodedSource = WebAssemblyRuntime.EscapeJs("" + args.NewValue);
				var js = $"element.controls = true; element.src=\"{encodedSource}\";";
				player.ExecuteJavascript(js);
			}
		}
	}
}

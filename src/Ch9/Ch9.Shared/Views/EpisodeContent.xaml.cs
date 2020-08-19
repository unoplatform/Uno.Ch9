﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Ch9
{
    public sealed partial class EpisodeContent : UserControl
    {
        public EpisodeContent()
        {
            this.InitializeComponent();

            App.Instance.Suspending += OnAppSuspended;
        }

        private void OnAppSuspended(object sender, SuspendingEventArgs e)
        {
#if !__WASM__
			this.MediaPlayer.MediaPlayer?.Pause();
#endif
		}
    }
}

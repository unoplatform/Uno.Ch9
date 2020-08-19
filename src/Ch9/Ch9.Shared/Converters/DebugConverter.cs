﻿using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Ch9
{
	/// <summary>
	/// Use this converter to debug data bindings in your xaml.
	/// </summary>
	public class DebugConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			// Put a breakpoint here to inspect values from the ViewModel to the View.
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			// Put a breakpoint here to inspect values from the View to the ViewModel.
			return value;
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using Uno.Extensions.Specialized;
using System.Globalization;

namespace Ch9
{
	public class FromNullableBoolToReverseBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (parameter != null)
			{
				throw new ArgumentException($"This converter does not use any parameters. You should remove \"{parameter}\" passed as parameter.");
			}

			if (value != null && !(value is bool))
			{
				throw new ArgumentException($"Value must either be null or of type bool. Got {value} ({value.GetType().FullName})");
			}

			var valueToConvert = value != null && System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);

			return !valueToConvert;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			// Same as Convert, except it should never be null
			if (value == null)
			{
				throw new InvalidOperationException("Since results should never be null, reverse conversion does not support null values");
			}

			return Convert(value, targetType, parameter, language);
		}
	}
}

using System;
using System.Collections;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Ch9
{
	/// <summary>
	/// This converter outputs a visibility value based on the presence of any items in an enumerable
	///
	/// VisibilityOnEnumerableHasAny (VisibilityOnEnumerableHasAny) : The visibility that should be returned
	/// when the enumerable has items.
	///
	/// By default, VisibilityOnEnumerableHasAny is set to Visible.
	///
	/// This may be used to show or hide a list based on the presence or absence of data.
	/// </summary>
	public class FromEnumerableHasAnyToVisibilityConverter : IValueConverter
	{
		public FromEnumerableHasAnyToVisibilityConverter()
		{
			VisibilityOnEnumerableHasAny = Visibility.Visible;
			VisibilityOnEnumerableNull = Visibility.Visible;
		}

		public Visibility VisibilityOnEnumerableHasAny { get; set; }

		public Visibility VisibilityOnEnumerableNull { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (parameter != null)
			{
				throw new ArgumentException($"This converter does not use any parameters. You should remove \"{parameter}\" passed as parameter.");
			}

			var inverse = VisibilityOnEnumerableHasAny == Visibility.Collapsed;

			var visibilityOnTrue = (!inverse) ? Visibility.Visible : Visibility.Collapsed;
			var visibilityOnFalse = (!inverse) ? Visibility.Collapsed : Visibility.Visible;

			var enumerableValue = value as IEnumerable;

			if (value != null && enumerableValue == null)
			{
				throw new ArgumentException($"Converter value (of type {value.GetType().FullName}) needs to be an IEnumerable.");
			}

			if (value == null)
			{
				return VisibilityOnEnumerableNull;
			}

			var valueToConvert = enumerableValue?.Cast<object>().Any() ?? false;

			return valueToConvert ? visibilityOnTrue : visibilityOnFalse;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return value;
		}
	}
}

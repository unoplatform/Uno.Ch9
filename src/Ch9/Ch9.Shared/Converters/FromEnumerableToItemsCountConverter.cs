using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;
using Uno.Extensions.Specialized;

namespace Ch9
{
    public class FromEnumerableToItemsCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var enumerableValue = value as IEnumerable;

            if (value != null && enumerableValue == null)
            {
                throw new ArgumentException($"Converter value (of type {value.GetType().FullName}) needs to be an IEnumerable.");
            }

            return value == null ? 0 : enumerableValue.Count();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}

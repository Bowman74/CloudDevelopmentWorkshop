using System;
using System.Globalization;
using Xamarin.Forms;

namespace Notes.Client.ValueConverters
{
    public class DescriptionValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return " ";
            string startValue = value.ToString().Replace("\r", " ");
            string endValue = startValue.Substring(0, startValue.Length >= 50 ? 50: startValue.Length);
            return endValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

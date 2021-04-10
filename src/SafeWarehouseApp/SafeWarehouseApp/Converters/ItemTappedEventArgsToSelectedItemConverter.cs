using System;
using System.Globalization;
using Xamarin.Forms;

namespace SafeWarehouseApp.Converters
{
    public class ItemTappedEventArgsToSelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var eventArgs = (ItemTappedEventArgs)value;
            return eventArgs.Item;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
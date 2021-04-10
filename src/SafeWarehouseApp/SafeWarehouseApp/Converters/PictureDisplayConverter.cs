using System;
using System.Globalization;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Converters
{
    public class PictureDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (string) value;
            var page = (Page) parameter;
            var viewModel = (BaseViewModel) page.BindingContext;
            var mediaService = viewModel.MediaService;
            return mediaService.GetMediaItemPathAsync(id).Result!;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
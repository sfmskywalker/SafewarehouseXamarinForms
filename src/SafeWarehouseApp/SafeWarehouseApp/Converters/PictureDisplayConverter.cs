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
            var id = (string?) value;
            var page = (Page) parameter;
            var viewModel = (BaseViewModel) page.BindingContext;
            var mediaService = viewModel.MediaService;
            var mediaItemPath = id != null ? mediaService.GetMediaItemPathAsync(id).Result : null;
            return mediaItemPath != null ? ImageSource.FromFile(mediaItemPath) : ImageSource.FromResource("SafeWarehouseApp.Assets.placeholder.png", typeof(PictureDisplayConverter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
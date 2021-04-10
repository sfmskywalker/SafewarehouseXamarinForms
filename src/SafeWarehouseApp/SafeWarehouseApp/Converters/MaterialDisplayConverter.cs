using System;
using System.Globalization;
using System.Linq;
using SafeWarehouseApp.Areas.Reports.ViewModels;
using SafeWarehouseApp.Areas.Reports.Views;
using Xamarin.Forms;

namespace SafeWarehouseApp.Converters
{
    public class MaterialDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var id = (string) value;
            var page = (EditDamagePage) parameter;
            var viewModel = (EditDamageViewModel) page.BindingContext;
            var material = viewModel.Materials.FirstOrDefault(x => x.Id == id);
            return material?.Name ?? id;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
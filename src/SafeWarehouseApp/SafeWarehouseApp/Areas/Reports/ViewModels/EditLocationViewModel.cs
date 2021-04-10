using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    [QueryProperty(nameof(LocationId), nameof(LocationId))]
    public class EditLocationViewModel : BaseViewModel
    {
        public string LocationId { get; set; } = default!;
        
        public async void OnAppearing()
        {
        }
    }
}

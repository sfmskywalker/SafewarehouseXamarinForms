using SafeWarehouseApp.Areas.DamageTypes.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.DamageTypes.Views
{
    public partial class ListDamageTypesPage : ContentPage
    {
        readonly ListDamageTypesViewModel _viewModel;

        public ListDamageTypesPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new ListDamageTypesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}
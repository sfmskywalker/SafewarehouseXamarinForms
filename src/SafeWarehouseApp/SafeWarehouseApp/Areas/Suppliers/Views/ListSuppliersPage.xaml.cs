using SafeWarehouseApp.Areas.Suppliers.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Suppliers.Views
{
    public partial class ListSuppliersPage : ContentPage
    {
        readonly ListSuppliersViewModel _viewModel;

        public ListSuppliersPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ListSuppliersViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}
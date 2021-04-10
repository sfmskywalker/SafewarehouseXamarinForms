using SafeWarehouseApp.Areas.Customers.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Customers.Views
{
    public partial class ListCustomersPage : ContentPage
    {
        readonly ListCustomersViewModel _viewModel;

        public ListCustomersPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ListCustomersViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}
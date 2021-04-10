using SafeWarehouseApp.Areas.Materials.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Materials.Views
{
    public partial class ListMaterialsPage : ContentPage
    {
        private readonly ListMaterialsViewModel _viewModel;

        public ListMaterialsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ListMaterialsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}
using SafeWarehouseApp.Areas.Materials.ViewModels;
using SafeWarehouseApp.Areas.Reports.ViewModels;

namespace SafeWarehouseApp.Areas.Reports.Views
{
    public partial class EditLocationPage
    {
        private readonly EditLocationViewModel _viewModel;
        
        public EditLocationPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new EditLocationViewModel();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}
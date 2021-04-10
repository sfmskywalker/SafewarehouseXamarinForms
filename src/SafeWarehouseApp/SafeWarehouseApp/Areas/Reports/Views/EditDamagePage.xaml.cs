using SafeWarehouseApp.Areas.Materials.ViewModels;
using SafeWarehouseApp.Areas.Reports.ViewModels;

namespace SafeWarehouseApp.Areas.Reports.Views
{
    public partial class EditDamagePage
    {
        private readonly EditDamageViewModel _viewModel;
        
        public EditDamagePage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new EditDamageViewModel();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}
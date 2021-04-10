using SafeWarehouseApp.Areas.Materials.ViewModels;

namespace SafeWarehouseApp.Areas.Materials.Views
{
    public partial class NewMaterialPage
    {
        private readonly NewMaterialViewModel _viewModel;
        
        public NewMaterialPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new NewMaterialViewModel();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}
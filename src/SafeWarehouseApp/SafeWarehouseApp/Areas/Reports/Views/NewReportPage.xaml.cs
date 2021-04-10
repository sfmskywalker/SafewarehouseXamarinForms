using SafeWarehouseApp.Areas.Materials.ViewModels;
using SafeWarehouseApp.Areas.Reports.ViewModels;

namespace SafeWarehouseApp.Areas.Reports.Views
{
    public partial class NewReportPage
    {
        private readonly NewReportViewModel _viewModel;
        
        public NewReportPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new NewReportViewModel();
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}
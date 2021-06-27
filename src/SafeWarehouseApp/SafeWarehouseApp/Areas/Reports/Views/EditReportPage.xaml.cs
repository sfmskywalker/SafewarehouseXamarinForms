using SafeWarehouseApp.Areas.Reports.ViewModels;

namespace SafeWarehouseApp.Areas.Reports.Views
{
    public partial class EditReportPage
    {
        private EditReportViewModel _viewModel;

        public EditReportPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new EditReportViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}
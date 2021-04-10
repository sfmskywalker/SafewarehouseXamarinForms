using SafeWarehouseApp.Areas.Reports.ViewModels;

namespace SafeWarehouseApp.Areas.Reports.Views
{
    public partial class ListReportsPage
    {
        private readonly ListReportsViewModel _viewModel;

        public ListReportsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ListReportsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}
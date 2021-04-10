using SafeWarehouseApp.Areas.Customers.ViewModels;

namespace SafeWarehouseApp.Areas.Customers.Views
{
    public partial class NewCustomerPage
    {
        private readonly NewCustomerViewModel _viewModel;

        public NewCustomerPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new NewCustomerViewModel();
        }

        protected override void OnAppearing() => _viewModel.OnAppearing();
    }
}
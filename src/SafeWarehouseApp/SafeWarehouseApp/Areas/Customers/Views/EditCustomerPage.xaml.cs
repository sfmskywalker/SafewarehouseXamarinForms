using SafeWarehouseApp.Areas.Customers.ViewModels;

namespace SafeWarehouseApp.Areas.Customers.Views
{
    public partial class EditCustomerPage
    {
        public EditCustomerPage()
        {
            InitializeComponent();
            BindingContext = new EditCustomerViewModel();
        }
    }
}
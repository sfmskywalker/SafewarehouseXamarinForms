using SafeWarehouseApp.Areas.Suppliers.ViewModels;

namespace SafeWarehouseApp.Areas.Suppliers.Views
{
    public partial class NewSupplierPage
    {
        public NewSupplierPage()
        {
            InitializeComponent();
            BindingContext = new NewSupplierViewModel();
        }
    }
}
using SafeWarehouseApp.Areas.Suppliers.ViewModels;

namespace SafeWarehouseApp.Areas.Suppliers.Views
{
    public partial class EditSupplierPage
    {
        public EditSupplierPage()
        {
            InitializeComponent();
            BindingContext = new EditSupplierViewModel();
        }
    }
}
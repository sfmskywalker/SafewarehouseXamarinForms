using SafeWarehouseApp.Areas.DamageTypes.ViewModels;

namespace SafeWarehouseApp.Areas.DamageTypes.Views
{
    public partial class EditDamageTypePage
    {
        public EditDamageTypePage()
        {
            InitializeComponent();
            BindingContext = new EditDamageTypeViewModel();
        }
    }
}
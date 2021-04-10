using SafeWarehouseApp.Areas.DamageTypes.ViewModels;

namespace SafeWarehouseApp.Areas.DamageTypes.Views
{
    public partial class NewDamageTypePage
    {
        public NewDamageTypePage()
        {
            InitializeComponent();
            BindingContext = new NewDamageTypeViewModel();
        }
    }
}
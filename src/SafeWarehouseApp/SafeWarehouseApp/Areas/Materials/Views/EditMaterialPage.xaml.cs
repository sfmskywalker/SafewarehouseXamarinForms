using SafeWarehouseApp.Areas.Materials.ViewModels;

namespace SafeWarehouseApp.Areas.Materials.Views
{
    public partial class EditMaterialPage
    {
        public EditMaterialPage()
        {
            InitializeComponent();
            BindingContext = new EditMaterialViewModel();
        }
    }
}
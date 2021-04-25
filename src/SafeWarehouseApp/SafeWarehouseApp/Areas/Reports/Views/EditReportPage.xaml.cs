using SafeWarehouseApp.Areas.Reports.ViewModels;

namespace SafeWarehouseApp.Areas.Reports.Views
{
    public partial class EditReportPage
    {
        public EditReportPage()
        {
            InitializeComponent();
            BindingContext = new EditReportViewModel();
        }
    }
}
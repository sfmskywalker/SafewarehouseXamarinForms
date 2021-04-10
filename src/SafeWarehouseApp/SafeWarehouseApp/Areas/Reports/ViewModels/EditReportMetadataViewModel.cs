using System.Threading.Tasks;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    public class EditReportMetadataViewModel : BaseViewModel
    {
        private Report _report = default!;

        public Report Report
        {
            get => _report;
            set => SetProperty(ref _report, value);
        }
    }
}
using System.Threading.Tasks;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.Services;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    [QueryProperty(nameof(ReportId), nameof(ReportId))]
    public class EditReportViewModel : BaseViewModel
    {
        private string _reportId = default!;
        private Report _report = default!;

        public EditReportViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            CreatePdfCommand = new Command(OnCreatePdf);
            PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
            EditReportLocationsViewModel = new EditReportLocationsViewModel();
            EditReportMetadataViewModel = new EditReportMetadataViewModel();
        }

        public string Id { get; private set; } = default!;
        public EditReportLocationsViewModel EditReportLocationsViewModel { get; set; }
        public EditReportMetadataViewModel EditReportMetadataViewModel { get; set; }

        public string ReportId
        {
            get => _reportId;
            set
            {
                _reportId = value;
                LoadItemId(value);
            }
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command CreatePdfCommand { get; }

        public async void LoadItemId(string itemId)
        {
            var report = await ReportStore.FindAsync(itemId);
            _report = report!;
            Id = report!.Id;
            EditReportLocationsViewModel.Report = report;
            EditReportMetadataViewModel.Report = report;
        }

        private bool ValidateSave() => true;
        private async Task CloseAsync() => await Shell.Current.GoToAsync("..", true);

        private async void OnCancel() => await CloseAsync();

        private async void OnSave()
        {
            var item = (await ReportStore.FindAsync(_reportId))!;
            await ReportStore.SaveAsync(item);
            await CloseAsync();
        }
        
        private async void OnCreatePdf()
        {
            var generator = GetService<IReportPdfGenerator>();
            var document = await generator.GenerateReportPdfAsync(_report);
        }
    }
}
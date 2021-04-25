using System;
using System.IO;
using System.Threading.Tasks;
using SafeWarehouseApp.Areas.Reports.Views;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.Services;
using SafeWarehouseApp.ViewModels;
using SkiaSharp;
using Xamarin.Essentials;
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
            var updatedBitmap = EditReportLocationsViewModel.CreateSchematicBitmap();
            using var image = SKImage.FromBitmap(updatedBitmap);
            var buffer = image.Encode(SKEncodedImageFormat.Png, 100).ToArray();
            var paintedSchematicMediaItem = _report.PaintedSchematicMediaId != null ? await MediaService.GetMediaItemAsync(_report.PaintedSchematicMediaId) : default;

            if (paintedSchematicMediaItem == null)
            {
                var schematicMediaItem = (await MediaService.GetMediaItemAsync(_report.SchematicMediaId))!;
                var extension = Path.GetExtension(schematicMediaItem.FileName);
                paintedSchematicMediaItem = await MediaService.CreateMediaItem(buffer, extension, schematicMediaItem.ContentType);
                await MediaItemStore.AddAsync(paintedSchematicMediaItem);
                _report.PaintedSchematicMediaId = paintedSchematicMediaItem.Id;
                await ReportStore.SaveAsync(_report);
            }
            else
            {
                await MediaService.SaveBufferAsync(buffer, paintedSchematicMediaItem.FileName);
            }
            
            await Shell.Current.GoToAsync($"{nameof(ReportPdfPage)}?{nameof(ReportPdfViewModel.ReportId)}={ReportId}", true);
        }
    }
}
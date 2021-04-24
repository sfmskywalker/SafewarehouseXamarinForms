using System;
using System.Threading.Tasks;
using Forms9Patch;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.Services;
using SafeWarehouseApp.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    [QueryProperty(nameof(ReportId), nameof(ReportId))]
    public class ReportPdfViewModel : BaseViewModel
    {
        private readonly Func<Task<ToFileResult?>> _generatePdfDocument;
        private string _reportId = default!;
        private Report _report = default!;
        private HtmlWebViewSource _reportHtmlSource = default!;

        public ReportPdfViewModel(Func<Task<ToFileResult?>> generatePdfDocument)
        {
            _generatePdfDocument = generatePdfDocument;
            SendCommand = new Command(OnSend);
        }

        public string Id { get; private set; } = default!;

        public string ReportId
        {
            get => _reportId;
            set
            {
                _reportId = value;
                LoadItemId(value);
            }
        }

        public HtmlWebViewSource ReportHtmlSource
        {
            get => _reportHtmlSource;
            set => SetProperty(ref _reportHtmlSource, value);
        }

        public Command SendCommand { get; }
        private IReportHtmlGenerator ReportHtmlGenerator => GetService<IReportHtmlGenerator>();

        public async void LoadItemId(string itemId)
        {
            var report = await ReportStore.FindAsync(itemId);
            _report = report!;
            Id = report!.Id;

            var html = await ReportHtmlGenerator.GenerateHtmlAsync(_report);
            
            ReportHtmlSource = new HtmlWebViewSource
            {
                BaseUrl = GetService<IBaseUrlProvider>().GetBaseUrl(),
                Html = html
            };
        }

        private async Task CloseAsync() => await Shell.Current.GoToAsync("..", true);

        private async void OnSend()
        {
            var fileResult = await _generatePdfDocument();

            if (fileResult != null)
            {
                var message = new EmailMessage
                {
                    Subject = "Schaderapport",
                    Body = "Zie bijlage.",
                };

                message.Attachments.Add(new EmailAttachment(fileResult.Result, "application/pdf"));

                try
                {
                    await Email.ComposeAsync(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            await CloseAsync();
        }
    }
}
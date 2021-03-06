using System;
using System.Collections.Generic;
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
        private Customer _customer = default!;
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
            var customer = await CustomerStore.FindAsync(report!.CustomerId);
            
            _report = report!;
            _customer = customer!;
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
                    Body = $"<p>Beste {_customer.ContactName},</p><p></p><p>Bijgevoegd vind je het schaderapport van {_report.Date.ToShortDateString()}.</p><p>Met vriendelijke groet,</p><p>Frans Hardus</p>",
                    BodyFormat = EmailBodyFormat.Html,
                    To = !string.IsNullOrWhiteSpace(_customer.Email) ? new List<string>{ _customer.Email } : new List<string>(0)
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
using System.Threading.Tasks;
using Forms9Patch;
using SafeWarehouseApp.Areas.Reports.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.Views
{
    public partial class ReportPdfPage
    {
        public ReportPdfPage()
        {
            InitializeComponent();
            WebViewPrintEffect.ApplyTo(WebView);
            BindingContext = new ReportPdfViewModel(GeneratePdfDocument);
        }

        private async Task<ToFileResult?> GeneratePdfDocument()
        {
            if (ToPdfService.IsAvailable)
            {
                var pdfResult = await WebView.ToPdfAsync("rapport");

                if (pdfResult.IsError)
                    using (Toast.Create("PDF Failure", pdfResult.Result))
                    {
                    }
                else
                {
                    return pdfResult;
                }
            }
            else
                using (Toast.Create(null, "PDF Export is not available on this device"))
                {
                }

            return null;
        }
    }
}
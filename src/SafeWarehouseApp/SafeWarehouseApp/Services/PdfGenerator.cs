using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SafeWarehouseApp.Services
{
    public class PdfGenerator : IPdfGenerator
    {
        public async Task<PdfDocument> GeneratePdfAsync(string template, object model, CancellationToken cancellationToken = default)
        {
            return new PdfDocument();
        }
    }

    public record PdfDocument();
}
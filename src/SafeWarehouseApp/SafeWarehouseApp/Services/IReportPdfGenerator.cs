using System.Threading;
using System.Threading.Tasks;
using SafeWarehouseApp.Models;

namespace SafeWarehouseApp.Services
{
    public interface IReportPdfGenerator
    {
        Task<PdfDocument> GenerateReportPdfAsync(Report report, CancellationToken cancellationToken = default);
    }
}
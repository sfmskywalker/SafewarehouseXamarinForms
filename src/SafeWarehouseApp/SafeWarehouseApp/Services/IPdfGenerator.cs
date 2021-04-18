using System.Threading;
using System.Threading.Tasks;

namespace SafeWarehouseApp.Services
{
    public interface IPdfGenerator
    {
        Task<PdfDocument> GeneratePdfAsync(string template, object model, CancellationToken cancellationToken = default);
    }
}
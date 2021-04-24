using System.Threading;
using System.Threading.Tasks;
using SafeWarehouseApp.Models;

namespace SafeWarehouseApp.Services
{
    public interface IReportHtmlGenerator
    {
        Task<string> GenerateHtmlAsync(Report report, CancellationToken cancellationToken = default);
    }
}
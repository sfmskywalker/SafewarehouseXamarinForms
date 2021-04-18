using System.Threading.Tasks;

namespace SafeWarehouseApp.Services
{
    public interface IActionSheetService
    {
        Task<string> ShowActionSheet(string title, string? cancel, string? destruction, params string[] buttons);
    }
}
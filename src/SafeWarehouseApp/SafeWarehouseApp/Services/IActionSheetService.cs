using System.Threading.Tasks;

namespace SafeWarehouseApp.Services
{
    public interface IActionSheetService
    {
        Task<string> ShowActionSheet(string title, string? cancel, string? destruction, string[] buttons);
    }

    public static class ActionSheetServiceExtensions
    {
        public static Task<string> ShowActionSheet(this IActionSheetService actionSheetService, string title, params string[] buttons) => actionSheetService.ShowActionSheet(title, default, default, buttons);
    }
}
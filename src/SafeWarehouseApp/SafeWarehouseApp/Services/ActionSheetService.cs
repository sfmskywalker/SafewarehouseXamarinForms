using System.Threading.Tasks;
using Xamarin.Forms;

namespace SafeWarehouseApp.Services
{
    public class ActionSheetService : IActionSheetService
    {
        private static Page CurrentMainPage => Application.Current.MainPage;
        public async Task<string> ShowActionSheet(string title, string? cancel, string? destruction, string[] buttons) => await CurrentMainPage.DisplayActionSheet(title, cancel, destruction, buttons);
    }
}
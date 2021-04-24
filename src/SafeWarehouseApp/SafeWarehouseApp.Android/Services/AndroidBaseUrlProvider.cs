using SafeWarehouseApp.Droid.Services;
using SafeWarehouseApp.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidBaseUrlProvider))]
namespace SafeWarehouseApp.Droid.Services
{
    public class AndroidBaseUrlProvider : IBaseUrlProvider
    {
        public string GetBaseUrl() => "file:///android_asset/";
    }
}
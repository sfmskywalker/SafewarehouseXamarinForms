using SafeWarehouseApp.Droid.Services;
using SafeWarehouseApp.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidMediaGallery))]
namespace SafeWarehouseApp.Droid.Services
{
    public class AndroidMediaGallery : IMediaGallery
    {
        public string Directory => Android.OS.Environment.DirectoryPictures;
    }
}
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Microsoft.Extensions.DependencyInjection;
using SafeWarehouseApp.Droid.Services;
using SafeWarehouseApp.Services;

namespace SafeWarehouseApp.Droid
{
    [Activity(
        Label = "SafeWarehouseApp",
        Icon = "@mipmap/icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize, ScreenOrientation = ScreenOrientation.FullSensor
    )]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Forms9Patch.Droid.Settings.Initialize(this);

            var app = new App();
            var services = new ServiceCollection();

            services.AddSingleton<IBaseUrlProvider, AndroidBaseUrlProvider>();
            services.AddSingleton<IMediaGallery, AndroidMediaGallery>();
            app.ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();
            App.Services = serviceProvider;

            app.BeforeStart();
            LoadApplication(app);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
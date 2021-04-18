using System;
using System.Globalization;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SafeWarehouseApp.Mapping;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.Persistence.Stores;
using SafeWarehouseApp.Services;

namespace SafeWarehouseApp
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();
            Services = CreateServices();
            MainPage = new AppShell();
        }

        public static IServiceProvider Services { get; private set; } = default!;

        private IServiceProvider CreateServices()
        {
            var services = new ServiceCollection();
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "veiligmagazijn.db");

            services
                .AddPooledDbContextFactory<SafeWarehouseContext>(options => options.UseSqlite($"Data Source={dbPath};Cache=Shared;"))
                .AddAutoMapper(typeof(AutoMapperProfile).Assembly)
                .AddSingleton<IStore<DamageType>, EntityFrameworkStore<DamageType>>()
                .AddSingleton<IStore<Supplier>, EntityFrameworkStore<Supplier>>()
                .AddSingleton<IStore<Material>, EntityFrameworkStore<Material>>()
                .AddSingleton<IStore<Customer>, CustomerStore>()
                .AddSingleton<IStore<Report>, ReportStore>()
                .AddSingleton<IStore<MediaItem>, EntityFrameworkStore<MediaItem>>()
                .AddSingleton<IMediaService, MediaService>()
                .AddSingleton<IActionSheetService, ActionSheetService>()
                .AddSingleton<IPdfGenerator, PdfGenerator>()
                .AddSingleton<IReportPdfGenerator, ReportPdfGenerator>();

            return services.BuildServiceProvider();
        }

        protected override void OnStart()
        {
            using var scope = Services.CreateScope();
            var factory = Services.GetRequiredService<IDbContextFactory<SafeWarehouseContext>>();
            using var dbContext = factory.CreateDbContext();
            dbContext.Database.Migrate();
            
            var nl = CultureInfo.CreateSpecificCulture("nl-NL");
            CultureInfo.DefaultThreadCurrentCulture = nl;
            CultureInfo.DefaultThreadCurrentUICulture = nl;
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
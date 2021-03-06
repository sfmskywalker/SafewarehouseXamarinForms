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
            MainPage = new AppShell();
        }

        public static IServiceProvider Services { get; set; } = default!;

        public void ConfigureServices(IServiceCollection services)
        {
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
                .AddSingleton<IReportHtmlGenerator, ReportHtmlGenerator>();
        }

        public void BeforeStart()
        {
            MigrateDb();
        }

        protected override void OnStart()
        {
            var nl = CultureInfo.CreateSpecificCulture("nl-NL");
            CultureInfo.DefaultThreadCurrentCulture = nl;
            CultureInfo.DefaultThreadCurrentUICulture = nl;
        }

        private void MigrateDb()
        {
            using var scope = Services.CreateScope();
            var factory = Services.GetRequiredService<IDbContextFactory<SafeWarehouseContext>>();
            using var dbContext = factory.CreateDbContext();
            dbContext.Database.Migrate();   
        }
    }
}
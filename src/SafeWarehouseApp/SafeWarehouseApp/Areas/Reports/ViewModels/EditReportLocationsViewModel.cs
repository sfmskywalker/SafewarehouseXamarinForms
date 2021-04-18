using System;
using SafeWarehouseApp.Areas.Reports.Views;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Services;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    public class EditReportLocationsViewModel : BaseViewModel
    {
        private Report _report = default!;
        private string _schematicImagePath = default!;

        public EditReportLocationsViewModel()
        {
            AddLocation = new Command<Point>(OnAddLocationAsync);
            EditLocation = new Command<Location>(OnEditLocationAsync);
            ShowActionSheet = new Command<Location>(OnShowActionSheet);
            SaveChanges = new Command(OnSaveChangesAsync);
        }

        public Report Report
        {
            get => _report;
            set
            {
                SetProperty(ref _report, value);
                LoadSchematicAsync();
            }
        }

        public string SchematicImagePath
        {
            get => _schematicImagePath;
            set => SetProperty(ref _schematicImagePath, value);
        }

        public Command<Point> AddLocation { get; }
        public Command<Location> EditLocation { get; }
        public Command<Location> ShowActionSheet { get; set; }
        public Command SaveChanges { get; }

        private async void LoadSchematicAsync()
        {
            if (Report == null!)
                return;
            
            SchematicImagePath = (await MediaService.GetMediaItemPathAsync(Report.SchematicMediaId))!;
        }
        
        private async void OnAddLocationAsync(Point position)
        {
            var location = new Location
            {
                Id = Guid.NewGuid().ToString("N"),
                Left = (int)position.X,
                Top = (int)position.Y,
                Radius = 100,
                Number = Report!.Locations.Count + 1
            };
            
            Report.Locations.Add(location);
            await ReportStore.UpdateAsync(Report);
            OnEditLocationAsync(location);
        }
        
        private async void OnEditLocationAsync(Location location)
        {
            await Shell.Current.GoToAsync($"{nameof(EditLocationPage)}?{nameof(EditLocationViewModel.ReportId)}={Report.Id}&{nameof(EditLocationViewModel.LocationId)}={location.Id}", true);    
        }
        
        private async void OnShowActionSheet(Location location)
        {
            var cancelAction = "Annuleren";
            var deleteAction = "Verwijderen";
            var action = await GetService<IActionSheetService>().ShowActionSheet($"Locatie {location.Number}", cancelAction, deleteAction);

            if (action == deleteAction)
            {
                Report.Locations.Remove(location);
                SaveChanges.Execute(null);
            }
        }

        private async void OnSaveChangesAsync() => await ReportStore.UpdateAsync(Report);
    }
}
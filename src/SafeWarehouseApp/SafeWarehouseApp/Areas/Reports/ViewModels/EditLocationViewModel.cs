using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Areas.Reports.Views;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    [QueryProperty(nameof(ReportId), nameof(ReportId))]
    [QueryProperty(nameof(LocationId), nameof(LocationId))]
    public class EditLocationViewModel : BaseViewModel
    {
        private string _reportId = default!;
        private string _locationId = default!;
        private Report _report = default!;
        private Location _location = default!;
        private string? _description;

        public EditLocationViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            AddDamageCommand = new Command(OnAddDamage);
            DeleteDamageCommand = new Command<DamageSummaryViewModel>(OnDeleteDamage);
            DamageSelected = new Command<DamageSummaryViewModel>(OnDamageSelected);
        }

        public ObservableCollection<DamageSummaryViewModel> Damages { get; set; } = new();

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command AddDamageCommand { get; }
        public Command<DamageSummaryViewModel> DeleteDamageCommand { get; set; }
        public Command<DamageSummaryViewModel> DamageSelected { get; }

        public string ReportId
        {
            get => _reportId;
            set
            {
                SetProperty(ref _reportId, value);
                LoadEntitiesAsync();
            }
        }

        public string LocationId
        {
            get => _locationId;
            set
            {
                SetProperty(ref _locationId, value);
                LoadEntitiesAsync();
            }
        }

        public Location Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }
        
        public string? Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public async void OnAppearing()
        {
        }

        private async void LoadEntitiesAsync()
        {
            if (ReportId == null! || LocationId == null!)
                return;

            _report = (await ReportStore.FindAsync(ReportId))!;
            _location = _report.Locations.First(x => x.Id == LocationId);
            Description = _location.Description;
            LoadDamagesAsync(_location);
        }

        private async void LoadDamagesAsync(Location location)
        {
            var damages = location.Damages;
            var damageTypes = (await DamageTypeStore.ListAsync()).ToDictionary(x => x.Id);
            
            string GetDamageTitle(Damage damage)
            {
                if(damage.DamageTypeId == null)
                    return damage.Description ?? "";
                
                var damageType = damageTypes.TryGet(damage.DamageTypeId);
                
                if (damageType == null)
                    return damage.Description ?? "";

                if (string.IsNullOrWhiteSpace(damage.Description))
                    return damageType.Name;

                return $"{damageType.Name}. {damage.Description}";
            }

            var models = damages.Select(x => new DamageSummaryViewModel(x.Id, x.Number, GetDamageTitle(x))).ToList();
            Damages.SetItems(models);
        }

        private bool ValidateSave() => true;
        private async Task CloseAsync() => await Shell.Current.GoToAsync("..", true);
        
        private async Task SaveAsync()
        {
            _location.Description = Description?.Trim();
            await ReportStore.UpdateAsync(_report);
        }

        private void OnDeleteDamage(DamageSummaryViewModel model)
        {
            var location = _location;
            var damage = location.Damages.First(x => x.Id == model.Id);
            location.Damages.Remove(damage);
            LoadDamagesAsync(_location);
        }

        private async void OnDamageSelected(DamageSummaryViewModel model)
        {
            var damage = _location.Damages.First(x => x.Id == model.Id);
            await OnEditDamage(damage);
        }

        private async void OnAddDamage()
        {
            var damage = new Damage
            {
                Id = Guid.NewGuid().ToString("N"),
                Number = _location.Damages.Count + 1
            };

            _location.Damages.Add(damage);
            await SaveAsync();
            await OnEditDamage(damage);
        }
        
        private async Task OnEditDamage(Damage damage)
        {
            await SaveAsync();
            await Shell.Current.GoToAsync($"{nameof(EditDamagePage)}?{nameof(EditDamageViewModel.ReportId)}={ReportId}&{nameof(EditDamageViewModel.LocationId)}={LocationId}&{nameof(EditDamageViewModel.DamageId)}={damage.Id}", true);
        }

        private async void OnCancel() => await CloseAsync();

        private async void OnSave()
        {
            await SaveAsync();
            await CloseAsync();
        }
    }
}
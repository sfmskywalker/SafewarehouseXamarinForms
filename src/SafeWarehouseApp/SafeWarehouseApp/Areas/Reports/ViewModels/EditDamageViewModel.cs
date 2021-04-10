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
    [QueryProperty(nameof(DamageId), nameof(DamageId))]
    public class EditDamageViewModel : BaseViewModel
    {
        private string _reportId = default!;
        private string _locationId = default!;
        private string _damageId = default!;
        private Damage _damage = default!;
        private DamageType? _damageType;
        private Report _report = default!;
        private Location _location = default!;
        
        public EditDamageViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            AddRequiredMaterialCommand = new Command(OnAddRequiredMaterial);
            DeleteRequiredMaterialCommand = new Command<RequiredMaterial>(OnDeleteRequiredMaterial);
            AddDamagePictureCommand = new Command(OnAddPicture);
            DeleteDamagePictureCommand = new Command<DamagePicture>(OnDeleteDamagePicture);
        }
        
        public IList<DamagePictureSummaryViewModel> Pictures { get; set; } = new List<DamagePictureSummaryViewModel>();
        public IList<Material> Materials { get; private set; } = new List<Material>();

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command AddRequiredMaterialCommand { get; }
        public Command<RequiredMaterial> DeleteRequiredMaterialCommand { get; set; }
        public Command AddDamagePictureCommand { get; }
        public Command<DamagePicture> DeleteDamagePictureCommand { get; set; }

        public ObservableCollection<DamageType> DamageTypes { get; } = new();
        public ObservableCollection<string> MaterialIds { get; } = new();
        public ObservableCollection<RequiredMaterial> RequiredMaterials { get; } = new();
        public ObservableCollection<DamagePicture> DamagePictures { get; } = new();

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
        
        public string DamageId
        {
            get => _damageId;
            set
            {
                SetProperty(ref _damageId, value);
                LoadEntitiesAsync();
            }
        }

        public DamageType? SelectedDamageType
        {
            get => _damageType;
            set => SetProperty(ref _damageType, value);
        }

        public async void OnAppearing()
        {
        }

        private async void LoadEntitiesAsync()
        {
            if (ReportId == null! || LocationId == null! || DamageId == null!)
                return;

            _report = (await ReportStore.FindAsync(ReportId))!;
            _location = _report.Locations.First(x => x.Id == LocationId);
            _damage = _location.Damages.First(x => x.Id == DamageId);

            LoadDamageTypesAsync();
            LoadMaterialsAsync();
            SelectedDamageType = DamageTypes.FirstOrDefault(x => x.Id == _damage.DamageTypeId);
            RequiredMaterials.SetItems(_damage.RequiredMaterials);
            DamagePictures.SetItems(_damage.Pictures);
        }

        private async void LoadDamageTypesAsync()
        {
            var damageTypes = await DamageTypeStore.ListAsync();
            DamageTypes.SetItems(damageTypes);
        }
        
        private async void LoadMaterialsAsync()
        {
            var materials = await MaterialStore.ListAsync();
            Materials = materials.ToList();
            MaterialIds.SetItems(Materials.Select(x => x.Id));
        }

        private bool ValidateSave() => true;
        private async Task CloseAsync() => await Shell.Current.GoToAsync("..", true);

        private void OnDeleteRequiredMaterial(RequiredMaterial requiredMaterial)
        {
        }

        private void OnAddRequiredMaterial()
        {
            var requiredMaterial = new RequiredMaterial
            {
                Quantity = 1,
            };
            
            RequiredMaterials.Add(requiredMaterial);
        }

        private void OnDeleteDamagePicture(DamagePicture damagePicture)
        {
            _damage.Pictures.Remove(damagePicture);
        }
        
        private async void OnAddPicture()
        {
            var damagePicture = new DamagePicture
            {
                Number = _damage.Pictures.Count + 1,
                PictureId = Guid.NewGuid().ToString("N")
            };
            
            DamagePictures.Add(damagePicture);
        }

        private async void OnCancel() => await CloseAsync();

        private async void OnSave()
        {
            _damage.RequiredMaterials = RequiredMaterials.ToList();
            _damage.Pictures = DamagePictures.ToList();
            _damage.DamageTypeId = SelectedDamageType?.Id;
            await ReportStore.SaveAsync(_report);
            await CloseAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Areas.Reports.Views;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.Services;
using SafeWarehouseApp.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Location = SafeWarehouseApp.Models.Location;

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
            DeleteDamagePictureCommand = new Command<DamagePictureViewModel>(OnDeleteDamagePicture);
            SelectPictureCommand = new Command<DamagePictureViewModel>(OnSelectPicture);
        }

        public IList<DamagePictureSummaryViewModel> Pictures { get; set; } = new List<DamagePictureSummaryViewModel>();
        public IList<Material> Materials { get; private set; } = new List<Material>();

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command AddRequiredMaterialCommand { get; }
        public Command<RequiredMaterial> DeleteRequiredMaterialCommand { get; set; }
        public Command AddDamagePictureCommand { get; }
        public Command<DamagePictureViewModel> DeleteDamagePictureCommand { get; set; }
        public Command<DamagePictureViewModel> SelectPictureCommand { get; set; }

        public ObservableCollection<DamageType> DamageTypes { get; } = new();
        public ObservableCollection<string> MaterialIds { get; } = new();
        public ObservableCollection<RequiredMaterial> RequiredMaterials { get; } = new();
        public ObservableCollection<DamagePictureViewModel> DamagePictures { get; } = new();

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

        private IActionSheetService ActionSheetService => GetService<IActionSheetService>();

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
            DamagePictures.SetItems(_damage.Pictures.Select(x => new DamagePictureViewModel(x.Number, x.PictureId, x.Description)));
        }

        private async void LoadDamageTypesAsync()
        {
            var damageTypes = (await DamageTypeStore.ListAsync()).OrderBy(x => x.Name).ToList();
            DamageTypes.SetItems(damageTypes);
        }

        private async void LoadMaterialsAsync()
        {
            var customerId = _report.CustomerId;
            var customer = (await CustomerStore.FindAsync(customerId))!;
            var supplierIds = customer.Suppliers;
            var materials = await MaterialStore.FindManyAsync(x => supplierIds.Contains(x.SupplierId));
            Materials = materials.OrderBy(x => x.Name).ToList();
            MaterialIds.SetItems(Materials.Select(x => x.Id));
        }

        private bool ValidateSave() => true;
        private async Task CloseAsync() => await Shell.Current.GoToAsync("..", true);

        private async Task<MediaItem?> LoadPhotoAsync(FileResult? photo)
        {
            if (photo == null)
                return null;

            var mediaItem = await MediaService.CreateMediaItem(photo);
            mediaItem.Tag = ReportId;

            await MediaItemStore.AddAsync(mediaItem);
            return mediaItem;
        }

        private async Task SelectPhotoAsync(Func<Task<FileResult?>> picker, DamagePictureViewModel damagePicture)
        {
            try
            {
                var photo = await picker();
                var mediaItem = await LoadPhotoAsync(photo);

                if (mediaItem != null)
                    damagePicture.PictureId = mediaItem.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SelectPhotoAsync threw: {ex.Message}");
            }
        }

        private async Task BeginSelectPicture(DamagePictureViewModel damagePicture)
        {
            var cameraButton = "Camera";
            var libraryButton = "Foto bibliotheek";
            var action = await ActionSheetService.ShowActionSheet("Selecteer media bron", cameraButton, libraryButton);

            if (action == cameraButton)
                await SelectPhotoAsync(() => MediaPicker.CapturePhotoAsync(), damagePicture);
            else if (action == libraryButton)
                await SelectPhotoAsync(() => MediaPicker.PickPhotoAsync(), damagePicture);
        }

        private void OnDeleteRequiredMaterial(RequiredMaterial requiredMaterial) => RequiredMaterials.Remove(requiredMaterial);

        private void OnAddRequiredMaterial()
        {
            var requiredMaterial = new RequiredMaterial
            {
                Quantity = 1,
            };

            RequiredMaterials.Add(requiredMaterial);
        }

        private void OnDeleteDamagePicture(DamagePictureViewModel damagePicture) => DamagePictures.Remove(damagePicture);
        private async void OnSelectPicture(DamagePictureViewModel damagePicture) => await BeginSelectPicture(damagePicture);

        private async void OnAddPicture()
        {
            var damagePicture = new DamagePicture
            {
                Number = _damage.Pictures.Count + 1,
                PictureId = Guid.NewGuid().ToString("N")
            };

            var viewModel = new DamagePictureViewModel(damagePicture.Number, damagePicture.PictureId, damagePicture.Description);
            await BeginSelectPicture(viewModel);

            if (viewModel.PictureId != null)
                DamagePictures.Add(viewModel);
        }

        private async void OnCancel() => await CloseAsync();

        private async void OnSave()
        {
            var currentPictureIds = _damage.Pictures.Select(x => x.PictureId).ToList();
            var newPictureIds = DamagePictures.Select(x => x.PictureId);
            var removedPictureIds = currentPictureIds.Where(x => !newPictureIds.Contains(x)).Select(x => x!).ToList();

            await MediaService.DeleteManyByIdAsync(removedPictureIds);

            _damage.RequiredMaterials = RequiredMaterials.ToList();
            _damage.Pictures = DamagePictures.Select(x => x.ToModel()).ToList();
            _damage.DamageTypeId = SelectedDamageType?.Id;
            await ReportStore.SaveAsync(_report);
            await CloseAsync();
        }
    }
}
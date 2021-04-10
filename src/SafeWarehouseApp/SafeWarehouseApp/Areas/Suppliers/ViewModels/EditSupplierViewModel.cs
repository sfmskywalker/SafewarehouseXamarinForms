using System.Threading.Tasks;
using SafeWarehouseApp.Areas.Materials.ViewModels;
using SafeWarehouseApp.Areas.Materials.Views;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Suppliers.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class EditSupplierViewModel : BaseViewModel
    {
        private string _itemId = default!;
        private string _supplierName = default!;

        public EditSupplierViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            ManageMaterialsCommand = new Command(OnManageMaterials);
            PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
        }

        public string Id { get; private set; } = default!;

        public string SupplierName
        {
            get => _supplierName;
            set => SetProperty(ref _supplierName, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command ManageMaterialsCommand { get; }

        public string ItemId
        {
            get => _itemId;
            set
            {
                _itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            var item = await SupplierStore.FindAsync(itemId);
            Id = item!.Id;
            SupplierName = item.Name;
        }

        private async void OnCancel() => await CloseAsync();

        private async void OnSave()
        {
            var item = await SupplierStore.FindAsync(_itemId);
            item!.Name = SupplierName.Trim();
            await SupplierStore.SaveAsync(item);
            await CloseAsync();
        }
        
        private async void OnManageMaterials()
        {
            await Shell.Current.Navigation.PopToRootAsync(false);
            await Shell.Current.GoToAsync($"//{nameof(ListMaterialsPage)}?{nameof(ListMaterialsViewModel.SupplierId)}={ItemId}", true);
        }

        private bool ValidateSave() => !string.IsNullOrWhiteSpace(_supplierName);
        private async Task CloseAsync() => await Shell.Current.GoToAsync("..", true);
    }
}
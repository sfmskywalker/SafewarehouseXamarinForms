using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Materials.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class EditMaterialViewModel : BaseViewModel
    {
        private string _itemId = default!;
        private string _name = default!;
        private Supplier? _selectedSupplier;

        public EditMaterialViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
        }

        public string Id { get; private set; } = default!;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        
        public Supplier? SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);
        }

        public string ItemId
        {
            get => _itemId;
            set
            {
                _itemId = value;
                LoadItemId(value);
            }
        }
        
        public ObservableCollection<Supplier> Suppliers { get; } = new();
        
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public async void LoadItemId(string itemId)
        {
            var item = await MaterialStore.FindAsync(itemId);
            Suppliers.SetItems(await SupplierStore.ListAsync(q => q.OrderBy(x => x.Name)));
            Id = item!.Id;
            Name = item.Name;
            SelectedSupplier = Suppliers.FirstOrDefault(x => x.Id == item.SupplierId);
        }
        
        private bool ValidateSave() => !string.IsNullOrWhiteSpace(_name) && SelectedSupplier != null;
        private async Task CloseAsync() => await Shell.Current.GoToAsync("..", true);
        
        private async void OnCancel() => await CloseAsync();

        private async void OnSave()
        {
            var item = await MaterialStore.FindAsync(_itemId);
            item!.Name = Name.Trim();
            item.SupplierId = SelectedSupplier!.Id;
            await MaterialStore.SaveAsync(item);
            await CloseAsync();
        }
    }
}
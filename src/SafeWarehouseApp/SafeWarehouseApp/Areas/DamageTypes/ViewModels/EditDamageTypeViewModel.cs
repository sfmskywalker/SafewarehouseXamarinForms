using System.Threading.Tasks;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.DamageTypes.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class EditDamageTypeViewModel : BaseViewModel
    {
        private string _itemId = default!;
        private string _displayName = default!;

        public EditDamageTypeViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
        }

        public string Id { get; private set; } = default!;

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

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
            var item = await DamageTypeStore.FindAsync(itemId);
            Id = item!.Id;
            DisplayName = item.Name;
        }

        private async void OnCancel() => await CloseAsync();

        private async void OnSave()
        {
            var item = await DamageTypeStore.FindAsync(_itemId);
            item!.Name = DisplayName.Trim();
            await DamageTypeStore.SaveAsync(item);
            await CloseAsync();
        }
        
        private bool ValidateSave() => !string.IsNullOrWhiteSpace(_displayName);
        private async Task CloseAsync() => await Shell.Current.GoToAsync("..", true);
    }
}
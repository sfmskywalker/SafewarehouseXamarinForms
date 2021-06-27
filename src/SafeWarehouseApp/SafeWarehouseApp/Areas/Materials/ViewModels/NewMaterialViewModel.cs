using System;
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
    [QueryProperty(nameof(SupplierId), nameof(SupplierId))]
    public class NewMaterialViewModel : BaseViewModel
    {
        private string _name = default!;
        private Supplier? _selectedSupplier;

        public NewMaterialViewModel()
        {
            SaveAndCloseCommand = new Command(OnSaveAndClose, ValidateSave);
            SaveAndNewCommand = new Command(OnSaveAndNew, ValidateSave);
            CancelCommand = new Command(OnCancel);

            PropertyChanged += (_, __) =>
            {
                SaveAndCloseCommand.ChangeCanExecute();
                SaveAndNewCommand.ChangeCanExecute();
            };
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        
        public string? SupplierId { get; set; }
        
        public Supplier? SelectedSupplier
        {
            get => _selectedSupplier;
            set => SetProperty(ref _selectedSupplier, value);
        }

        public ObservableCollection<Supplier> Suppliers { get; } = new();

        public Command SaveAndCloseCommand { get; }
        public Command SaveAndNewCommand { get; }
        public Command CancelCommand { get; }
        
        private bool ValidateSave() => !string.IsNullOrWhiteSpace(_name) && SelectedSupplier != null;

        private async Task SaveAsync()
        {
            var newItem = new Material
            {
                Id = Guid.NewGuid().ToString("N"),
                SupplierId = SelectedSupplier!.Id,
                Name = Name.Trim(),
            };

            await MaterialStore.AddAsync(newItem);
        }

        public async void OnAppearing()
        {
            var suppliers = (await SupplierStore.ListAsync(q => q.OrderBy(x => x.Name))).ToList();
            Suppliers.SetItems(suppliers);

            if (!string.IsNullOrWhiteSpace(SupplierId))
                SelectedSupplier = suppliers.FirstOrDefault(x => x.Id == SupplierId);
        }

        private async void OnCancel() => await CloseAsync();

        private async void OnSaveAndClose()
        {
            await SaveAsync();
            await CloseAsync();
        }

        private async void OnSaveAndNew()
        {
            await SaveAsync();
            Name = "";
        }

        private async Task CloseAsync() => await Shell.Current.GoToAsync("..");
    }
}

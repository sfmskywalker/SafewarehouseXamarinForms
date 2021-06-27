using System;
using System.Threading.Tasks;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Suppliers.ViewModels
{
    public class NewSupplierViewModel : BaseViewModel
    {
        private string _name = "";

        public NewSupplierViewModel()
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

        private bool ValidateSave() => !string.IsNullOrWhiteSpace(_name);

        public string SupplierName
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public Command SaveAndCloseCommand { get; }
        public Command SaveAndNewCommand { get; }
        public Command CancelCommand { get; }

        private async Task SaveAsync()
        {
            var newItem = new Supplier
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = SupplierName,
            };

            await SupplierStore.AddAsync(newItem);
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
            SupplierName = "";
        }

        private async Task CloseAsync() => await Shell.Current.GoToAsync("..");
    }
}

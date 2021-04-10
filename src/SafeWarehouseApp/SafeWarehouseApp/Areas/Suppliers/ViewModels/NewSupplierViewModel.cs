using System;
using System.Threading.Tasks;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Suppliers.ViewModels
{
    public class NewSupplierViewModel : BaseViewModel
    {
        private string _name;

        public NewSupplierViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave() => !string.IsNullOrWhiteSpace(_name);

        public string SupplierName
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel() => await CloseAsync();

        private async void OnSave()
        {
            var newItem = new Supplier
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = SupplierName,
            };

            await SupplierStore.AddAsync(newItem);
            await CloseAsync();
        }

        private async Task CloseAsync() => await Shell.Current.GoToAsync("..");
    }
}

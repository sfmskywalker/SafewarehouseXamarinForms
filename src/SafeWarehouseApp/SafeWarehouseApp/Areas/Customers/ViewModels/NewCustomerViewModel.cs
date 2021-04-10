using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Customers.ViewModels
{
    public class NewCustomerViewModel : BaseViewModel
    {
        private string _companyName = default!;
        private string? _contactName;
        private string _email = default!;
        private string? _city;
        private string? _address;
        private IList<SelectableItem<Supplier>> _suppliers = new List<SelectableItem<Supplier>>();

        public NewCustomerViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave() => !string.IsNullOrWhiteSpace(_companyName);

        public string CompanyName
        {
            get => _companyName;
            set => SetProperty(ref _companyName, value);
        }

        public string? ContactName
        {
            get => _contactName;
            set => SetProperty(ref _contactName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string? City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }

        public string? Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public IList<SelectableItem<Supplier>> Suppliers
        {
            get => _suppliers;
            set => SetProperty(ref _suppliers, value);
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public async void OnAppearing()
        {
            IsBusy = true;

            var suppliers = await SupplierStore.ListAsync(q => q.OrderBy(x => x.Name));
            Suppliers = suppliers.Select(x => new SelectableItem<Supplier>(x)).ToList();

            IsBusy = false;
        }

        private async void OnCancel() => await CloseAsync();

        private async void OnSave()
        {
            var newItem = new Customer
            {
                Id = Guid.NewGuid().ToString("N"),
                CompanyName = CompanyName.Trim(),
                ContactName = _contactName?.Trim(),
                Email = Email.Trim(),
                City = City?.Trim(),
                Address = Address?.Trim(),
                Suppliers = Suppliers.Where(x => x.IsSelected).Select(x => x.Item.Id).ToList()
            };

            await CustomerStore.AddAsync(newItem);
            await CloseAsync();
        }

        private async Task CloseAsync() => await Shell.Current.GoToAsync("..");
    }
}
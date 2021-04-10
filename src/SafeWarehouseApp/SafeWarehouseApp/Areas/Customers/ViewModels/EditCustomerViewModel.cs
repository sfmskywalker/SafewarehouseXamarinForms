using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Areas.Customers.Views;
using SafeWarehouseApp.Areas.Materials.ViewModels;
using SafeWarehouseApp.Areas.Materials.Views;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Customers.ViewModels
{
    [QueryProperty(nameof(CustomerId), nameof(CustomerId))]
    public class EditCustomerViewModel : BaseViewModel
    {
        private string _id = default!;
        private string _companyName = default!;
        private string? _contactName;
        private string _email = default!;
        private string? _city;
        private string? _address;
        private IList<SelectableItem<Supplier>> _suppliers = new List<SelectableItem<Supplier>>();
        
        public EditCustomerViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            ManageReportsCommand = new Command(OnManageReports);
            PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();
            Title = "Bewerk Klant";
        }

        public string Id { get; private set; } = default!;

        public string CompanyName
        {
            get => _companyName;
            set
            {
                SetProperty(ref _companyName, value);
                Title = _companyName;
            }
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

        public string CustomerId
        {
            get => _id;
            set
            {
                _id = value;
                LoadCustomerAsync(value);
            }
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command ManageReportsCommand { get; }

        public async void LoadCustomerAsync(string customerId)
        {
            IsBusy = true;
            var customer = await CustomerStore.FindAsync(customerId);
            IsBusy = false;

            Id = customer!.Id;
            CompanyName = customer.CompanyName;
            ContactName = customer.ContactName;
            Email = customer.Email;
            City = customer.City;
            Address = customer.Address;

            var suppliers = await SupplierStore.ListAsync(q => q.OrderBy(x => x.Name));
            Suppliers = suppliers.Select(x => new SelectableItem<Supplier>(x, customer.Suppliers.Contains(x.Id))).ToList();
        }

        private async void OnCancel() => await CloseAsync();

        private async void OnSave()
        {
            var customer = await CustomerStore.FindAsync(_id);
            customer!.CompanyName = CompanyName.Trim();
            customer.ContactName = ContactName?.Trim();
            customer.Email = Email.Trim();
            customer.City = City?.Trim();
            customer.Address = Address?.Trim();
            customer.Suppliers = Suppliers.Where(x => x.IsSelected).Select(x => x.Item.Id).ToList();
            await CustomerStore.SaveAsync(customer);
            await CloseAsync();
        }

        private async void OnManageReports()
        {
            OnSave();
            await Shell.Current.Navigation.PopToRootAsync(false);
            await Shell.Current.GoToAsync($"//{nameof(ListCustomersPage)}", true);
        }

        private bool ValidateSave() => !string.IsNullOrWhiteSpace(_companyName);
        private async Task CloseAsync() => await Shell.Current.GoToAsync("..", true);
    }
}
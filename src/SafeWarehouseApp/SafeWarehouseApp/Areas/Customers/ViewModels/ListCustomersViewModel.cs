using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Areas.Customers.Views;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Customers.ViewModels
{
    public class ListCustomersViewModel : BaseViewModel
    {
        public ObservableCollection<Customer> Customers { get; }
        public Command LoadCustomersCommand { get; }
        public Command AddCustomerCommand { get; }
        public Command DeleteCustomerCommand { get; }
        public Command<Customer> CustomerSelected { get; }

        public ListCustomersViewModel()
        {
            Title = "Klanten";
            Customers = new ObservableCollection<Customer>();
            LoadCustomersCommand = new Command(async () => await ExecuteLoadCustomersCommand());
            CustomerSelected = new Command<Customer>(OnCustomerSelected);
            AddCustomerCommand = new Command(OnAddCustomer);
            DeleteCustomerCommand = new Command<Customer>(OnDeleteCustomer);
        }

        async Task ExecuteLoadCustomersCommand()
        {
            IsBusy = true;

            try
            {
                var items = await CustomerStore.ListAsync(q => q.OrderBy(x => x.CompanyName));
                Customers.SetItems(items);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing() => IsBusy = true;

        private async void OnAddCustomer(object obj) => await Shell.Current.GoToAsync(nameof(NewCustomerPage));

        private async void OnDeleteCustomer(Customer item)
        {
            await CustomerStore.DeleteAsync(item);
            Customers.Remove(item);
        }

        private async void OnCustomerSelected(Customer? item)
        {
            if (item == null)
                return;
            
            await Shell.Current.GoToAsync($"{nameof(EditCustomerPage)}?{nameof(EditCustomerViewModel.CustomerId)}={item.Id}");
        }
    }
}
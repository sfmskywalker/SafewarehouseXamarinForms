using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Areas.Suppliers.Views;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Suppliers.ViewModels
{
    public class ListSuppliersViewModel : BaseViewModel
    {
        public ObservableCollection<Supplier> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command DeleteItemCommand { get; }
        public Command<Supplier> ItemSelected { get; }

        public ListSuppliersViewModel()
        {
            Title = "Leveranciers";
            Items = new ObservableCollection<Supplier>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemSelected = new Command<Supplier>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
            DeleteItemCommand = new Command<Supplier>(OnDeleteItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await SupplierStore.ListAsync(q => q.OrderBy(x => x.Name));
                
                foreach (var item in items) 
                    Items.Add(item);
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

        public void OnAppearing()
        {
            IsBusy = true;
        }
        
        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewSupplierPage));
        }
        
        private async void OnDeleteItem(Supplier item)
        {
            await SupplierStore.DeleteAsync(item);
            Items.Remove(item);
        }

        private async void OnItemSelected(Supplier? item)
        {
            if (item == null)
                return;
            
            await Shell.Current.GoToAsync($"{nameof(EditSupplierPage)}?{nameof(EditSupplierViewModel.ItemId)}={item.Id}");
        }
    }
}
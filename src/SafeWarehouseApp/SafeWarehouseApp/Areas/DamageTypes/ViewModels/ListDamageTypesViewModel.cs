using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Areas.DamageTypes.Views;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.DamageTypes.ViewModels
{
    public class ListDamageTypesViewModel : BaseViewModel
    {
        public ObservableCollection<DamageType> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command DeleteItemCommand { get; }
        public Command<DamageType> ItemSelected { get; }

        public ListDamageTypesViewModel()
        {
            Title = "Schadesoorten";
            Items = new ObservableCollection<DamageType>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemSelected = new Command<DamageType>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
            DeleteItemCommand = new Command<DamageType>(OnDeleteItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.SetItems(await DamageTypeStore.ListAsync(q => q.OrderBy(x => x.Name)));
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

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewDamageTypePage));
        }
        
        private async void OnDeleteItem(DamageType item)
        {
            await DamageTypeStore.DeleteAsync(item);
            Items.Remove(item);
        }

        private async void OnItemSelected(DamageType? item)
        {
            if (item == null)
                return;
            
            await Shell.Current.GoToAsync($"{nameof(EditDamageTypePage)}?{nameof(EditDamageTypeViewModel.ItemId)}={item.Id}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Areas.Materials.Views;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Materials.ViewModels
{
    [QueryProperty(nameof(SupplierId), nameof(SupplierId))]
    public class ListMaterialsViewModel : BaseViewModel
    {
        private IDictionary<string, Supplier>? _suppliers;
        private Supplier? _supplier;

        public ListMaterialsViewModel()
        {
            Title = "Materialen";
            Groups = new ObservableCollection<IGrouping<Supplier, Material>>();
            FilteredGroups = new ObservableCollection<IGrouping<Supplier, Material>>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemSelected = new Command<Material>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
            DeleteItemCommand = new Command<Material>(OnDeleteItem);
        }

        public ObservableCollection<IGrouping<Supplier, Material>> Groups { get; }
        public ObservableCollection<IGrouping<Supplier, Material>> FilteredGroups { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command DeleteItemCommand { get; }
        public Command<Material> ItemSelected { get; }

        public string? SupplierId
        {
            get => _supplier?.Id;
            set => LoadSupplier(value);
        }

        private async void LoadSupplier(string? id)
        {
            var suppliers = await GetSuppliersAsync();
            _supplier = string.IsNullOrWhiteSpace(id) ? default : suppliers.Values.First(x => x.Id == id);
            Title = _supplier == null ? "Materialen" : $"Materialen van {_supplier.Name}";
            await LoadAsync();
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;
            await LoadAsync();
        }

        public void OnAppearing()
        {
            IsBusy = true;
            _supplier = null;
            _suppliers = null;
        }

        private async Task<IDictionary<string, Supplier>> GetSuppliersAsync() => _suppliers ??= (await SupplierStore.ListAsync()).ToDictionary(x => x.Id);

        private async Task LoadAsync()
        {
            try
            {
                var suppliers = await GetSuppliersAsync();
                var materials = (await MaterialStore.ListAsync(q => q.OrderBy(x => x.Name))).ToList();

                var query =
                    from material in materials
                    let supplier = suppliers[material.SupplierId]
                    orderby supplier.Name
                    select (material, supplier);

                var groups = query.GroupBy(x => x.supplier, x => x.material).ToList();
                Groups.SetItems(groups);
                FilteredGroups.SetItems(_supplier != null ? materials.Where(x => x.SupplierId == _supplier.Id).GroupBy(x => _supplier) : groups);
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

        private async void OnAddItem()
        {
            var state = _supplier == null ? nameof(NewMaterialPage) : $"{nameof(NewMaterialPage)}?{nameof(NewMaterialViewModel.SupplierId)}={_supplier.Id}";
            await Shell.Current.GoToAsync(state);
        }

        private async void OnDeleteItem(Material item)
        {
            IsBusy = true;
            await MaterialStore.DeleteAsync(item);
            await LoadAsync();
        }

        private async void OnItemSelected(Material? item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(EditMaterialPage)}?{nameof(EditMaterialViewModel.ItemId)}={item.Id}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Areas.Materials.Views;
using SafeWarehouseApp.Areas.Reports.Views;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    [QueryProperty(nameof(CustomerId), nameof(CustomerId))]
    public class ListReportsViewModel : BaseViewModel
    {
        private Customer? _customer;

        public ListReportsViewModel()
        {
            Title = "Rapporten";
            Groups = new ObservableCollection<IGrouping<Customer, Report>>();
            FilteredGroups = new ObservableCollection<IGrouping<Customer, Report>>();
            LoadReportsCommand = new Command(async () => await OnLoadReports());
            ReportSelected = new Command<Report>(OnReportSelected);
            AddReportCommand = new Command(OnAddReport);
            DeleteReportCommand = new Command<Report>(OnDeleteReport);
        }

        public ObservableCollection<IGrouping<Customer, Report>> Groups { get; }
        public ObservableCollection<IGrouping<Customer, Report>> FilteredGroups { get; }
        public Command LoadReportsCommand { get; }
        public Command AddReportCommand { get; }
        public Command DeleteReportCommand { get; }
        public Command<Report> ReportSelected { get; }

        public string? CustomerId
        {
            get => _customer?.Id;
            set => LoadCustomerAsync(value);
        }

        private async void LoadCustomerAsync(string? id)
        {
            var customers = await GetCustomersAsync();
            _customer = string.IsNullOrWhiteSpace(id) ? default : customers.Values.First(x => x.Id == id);
            Title = _customer == null ? "Rapporten" : $"Rapporten van {_customer.CompanyName}";
            await LoadAsync();
        }

        async Task OnLoadReports()
        {
            IsBusy = true;
            await LoadAsync();
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        private async Task<IDictionary<string, Customer>> GetCustomersAsync() => (await CustomerStore.ListAsync()).ToDictionary(x => x.Id);

        private async Task LoadAsync()
        {
            try
            {
                var customers = await GetCustomersAsync();
                var reports = (await ReportStore.ListAsync(q => q.OrderByDescending(x => x.Date))).ToList();

                var query =
                    from report in reports
                    let customer = customers[report.CustomerId]
                    orderby customer.CompanyName
                    select (report, customer);

                var groups = query.GroupBy(x => x.customer, x => x.report).ToList();
                Groups.SetItems(groups);
                FilteredGroups.SetItems(_customer != null ? reports.Where(x => x.CustomerId == _customer.Id).GroupBy(x => _customer) : groups);
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

        private async void OnAddReport()
        {
            var state = _customer == null ? nameof(NewReportPage) : $"{nameof(NewReportPage)}?{nameof(NewReportViewModel.CustomerId)}={_customer.Id}";
            await Shell.Current.GoToAsync(state);
        }

        private async void OnDeleteReport(Report report)
        {
            IsBusy = true;
            await ReportStore.DeleteAsync(report);
            await MediaService.DeleteManyByTagAsync(report.Id);
            await LoadAsync();
        }

        private async void OnReportSelected(Report? item)
        {
            if (item == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(EditReportPage)}?{nameof(EditReportViewModel.ReportId)}={item.Id}");
        }
    }
}
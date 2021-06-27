using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.ViewModels;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    public class EditReportMetadataViewModel : BaseViewModel
    {
        private Report? _report;
        private string? _customerId;
        private Customer? _selectedCustomer;
        private string? _remarks;
        private DateTime? _reportDate;
        private DateTime? _nextExaminationBefore;

        public EditReportMetadataViewModel()
        {
            SaveChanges = new Command(OnSaveChangesAsync);
        }

        public Report Report
        {
            get => _report!;
            set
            {
                SetProperty(ref _report, value);
                CustomerId = value.CustomerId;
                Remarks = value.Remarks;
                ReportDate = value.Date;
                NextExaminationBefore = value.NextExaminationBefore;
            }
        }

        public string? CustomerId
        {
            get => _customerId;
            set => SetProperty(ref _customerId, value);
        }

        public string? Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        public DateTime? ReportDate
        {
            get => _reportDate;
            set => SetProperty(ref _reportDate, value);
        }

        public DateTime? NextExaminationBefore
        {
            get => _nextExaminationBefore;
            set => SetProperty(ref _nextExaminationBefore, value);
        }

        public Customer? SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        public ObservableCollection<Customer> Customers { get; } = new();

        public Command SaveChanges { get; }

        public async void OnAppearing()
        {
            var customers = (await CustomerStore.ListAsync(q => q.OrderBy(x => x.CompanyName))).ToList();
            Customers.SetItems(customers);

            if (!string.IsNullOrWhiteSpace(CustomerId))
                SelectedCustomer = customers.FirstOrDefault(x => x.Id == CustomerId);
        }

        private async Task SaveChangesAsync()
        {
            Report.CustomerId = SelectedCustomer?.Id;
            Report.Remarks = Remarks?.Trim();
            Report.Date = ReportDate ?? Report.Date;
            Report.NextExaminationBefore = NextExaminationBefore;
            await ReportStore.UpdateAsync(Report);
        }
        
        private async void OnSaveChangesAsync() => await SaveChangesAsync();
    }
}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SafeWarehouseApp.Areas.Reports.Views;
using SafeWarehouseApp.Extensions;
using SafeWarehouseApp.Models;
using SafeWarehouseApp.Persistence;
using SafeWarehouseApp.Services;
using SafeWarehouseApp.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SafeWarehouseApp.Areas.Reports.ViewModels
{
    [QueryProperty(nameof(CustomerId), nameof(CustomerId))]
    public class NewReportViewModel : BaseViewModel
    {
        private Customer? _selectedCustomer;

        public NewReportViewModel()
        {
            TakePictureCommand = new Command(OnTakePicture, ValidateSave);
            SelectPictureCommand = new Command(OnSelectPicture, ValidateSave);
            
            PropertyChanged += (_, __) =>
            {
                TakePictureCommand.ChangeCanExecute();
                SelectPictureCommand.ChangeCanExecute();
            };
        }

        public Command SelectPictureCommand { get; set; }
        public Command TakePictureCommand { get; set; }
        
        public string? CustomerId { get; set; }
        
        public Customer? SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        public ObservableCollection<Customer> Customers { get; } = new();

        public async void OnAppearing()
        {
            var customers = (await CustomerStore.ListAsync(q => q.OrderBy(x => x.CompanyName))).ToList();
            Customers.SetItems(customers);

            if (!string.IsNullOrWhiteSpace(CustomerId))
                SelectedCustomer = customers.FirstOrDefault(x => x.Id == CustomerId);
        }
        
        private bool ValidateSave() => SelectedCustomer != null;

        private async Task LoadPhotoAsync(FileResult? photo)
        {
            if (photo == null)
                return;
            
            var schematicMedia = await MediaService.CreateMediaItem(photo);
            var report = await CreateReportAsync(schematicMedia.Id);
            
            schematicMedia.Tag = report.Id;
            
            await MediaItemStore.AddAsync(schematicMedia);
            await GoEditReportAsync(report.Id);
        }
        
        private async Task SelectPhotoAsync(Func<Task<FileResult?>> picker)
        {
            try
            {
                var photo = await picker();
                await LoadPhotoAsync(photo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SelectPhotoAsync threw: {ex.Message}");
            }
        }
        
        private async Task<Report> CreateReportAsync(string schematicMediaId)
        {
            var report = new Report
            {
                Id = Guid.NewGuid().ToString("N"),
                Date = DateTime.Now,
                CustomerId = SelectedCustomer!.Id,
                SchematicMediaId = schematicMediaId
            };

            await ReportStore.AddAsync(report);
            return report;
        }

        private async Task GoEditReportAsync(string id)
        {
            await Shell.Current.Navigation.PopToRootAsync();
            await Shell.Current.GoToAsync($"{nameof(EditReportPage)}?{nameof(EditReportViewModel.ReportId)}={id}");
        }

        private async void OnTakePicture() => await SelectPhotoAsync(() => MediaPicker.CapturePhotoAsync());
        private async void OnSelectPicture() => await SelectPhotoAsync(() => MediaPicker.PickPhotoAsync());
    }
}
